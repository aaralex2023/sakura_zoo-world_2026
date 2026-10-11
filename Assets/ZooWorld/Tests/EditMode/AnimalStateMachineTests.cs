using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.Tests
{
    public sealed class AnimalStateMachineTests
    {
        private readonly List<Object> _objects = new();
        private AnimalStateMachine _machine;
        private AnimalContext _context;
        private AnimalStateId _start;
        private AnimalStateId _next;
        private GameObject _startObject;
        private StateLifecycleProbe _first;
        private StateLifecycleProbe _second;
        private StateLifecycleProbe _nextProbe;
        private AnimalVarId _contactFlag;

        [SetUp]
        public void SetUp()
        {
            GameObject root = new("Animal");
            _objects.Add(root);
            _machine = root.AddComponent<AnimalStateMachine>();
            Animal animal = root.AddComponent<Animal>();
            Rigidbody body = root.AddComponent<Rigidbody>();
            Collider collider = root.AddComponent<BoxCollider>();
            AnimalConfigProvider configs = root.AddComponent<AnimalConfigProvider>();
            AnimalRuntimeState runtime = new(root.transform, body, collider, new AnimalVars(),
                _machine.IsInState, _machine.ForceSwitchState, _machine.RequireState);
            _context = new AnimalContext(animal, runtime, configs);
            _startObject = CreateState(root, out _start);
            GameObject nextObject = CreateState(root, out _next);
            CreateState(root, out AnimalStateId dead);
            SetField(_machine, "_deadState", dead);
            _first = _startObject.AddComponent<StateLifecycleProbe>();
            _second = _startObject.AddComponent<StateLifecycleProbe>();
            _nextProbe = nextObject.AddComponent<StateLifecycleProbe>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (Object instance in _objects)
                Object.DestroyImmediate(instance);
            _objects.Clear();
        }

        [TestCase(typeof(AnimalStateSelectHeading))]
        [TestCase(typeof(AnimalStateSwitchByVariableTimer))]
        [TestCase(typeof(AnimalStateSwitchByVarFlag))]
        [TestCase(typeof(AnimalStateSwitchIfGrounded))]
        [TestCase(typeof(AnimalStateSwitchIfOutOfScreen))]
        public void UnregisteredTransitionTargetFailsBeforeEnteringStartState(System.Type componentType)
        {
            AnimalStateId missing = ScriptableObject.CreateInstance<AnimalStateId>();
            missing.name = "MissingTarget";
            _objects.Add(missing);
            Component transition = _startObject.AddComponent(componentType);
            SetField(transition, "_nextState", missing);

            var exception = Assert.Throws<System.InvalidOperationException>(() => _machine.Initialize(_context, _start));

            Assert.That(exception.Message, Does.Contain("MissingTarget"));
            Assert.That(_first.Enters, Is.Zero);
        }

        [Test]
        public void TransitionCanReferenceNodePreparedLater()
        {
            var flag = ScriptableObject.CreateInstance<AnimalVarId>();
            _objects.Add(flag);
            var transition = _startObject.AddComponent<AnimalStateSwitchByVarFlag>();
            SetField(transition, "_flagId", flag);
            SetField(transition, "_nextState", _next);

            _machine.Initialize(_context, _start);
            _context.State.Vars.Set(flag, 0d);
            _machine.LifeCycle(0.02f);

            Assert.That(_machine.IsInState(_next), Is.True);
        }

        [Test]
        public void MissingTimerIdInInactiveStateFailsDuringInitialization()
        {
            var transition = _nextProbe.gameObject.AddComponent<AnimalStateSwitchByVariableTimer>();
            SetField(transition, "_nextState", _start);

            var exception = Assert.Throws<System.InvalidOperationException>(() => _machine.Initialize(_context, _start));

            Assert.That(exception.Message, Does.Contain("no timer variable ID"));
            Assert.That(_first.Enters, Is.Zero);
        }

        [Test]
        public void MissingTimerValueInInactiveStateFailsDuringInitialization()
        {
            AnimalVarId timerId = CreateTimer(_nextProbe.gameObject, _start);
            timerId.name = "MissingDuration";

            var exception = Assert.Throws<System.InvalidOperationException>(() => _machine.Initialize(_context, _start));

            Assert.That(exception.Message, Does.Contain("MissingDuration"));
            Assert.That(_first.Enters, Is.Zero);
        }

        [TestCase(-1d)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void InvalidTimerDurationFailsDuringInitialization(double duration)
        {
            AnimalVarId timerId = CreateTimer(_nextProbe.gameObject, _start);
            _context.State.Vars.Set(timerId, duration);

            Assert.Throws<System.InvalidOperationException>(() => _machine.Initialize(_context, _start));
            Assert.That(_first.Enters, Is.Zero);
        }

        [TestCase(0d)]
        [TestCase(0.05d)]
        public void ConfiguredTimerTransitionsAfterItsDuration(double duration)
        {
            AnimalVarId timerId = CreateTimer(_startObject, _next);
            _context.State.Vars.Set(timerId, duration);
            _machine.Initialize(_context, _start);
            Assert.That(_machine.IsInState(_start), Is.True);
            if (duration > 0d)
            {
                _machine.LifeCycle(0.02f);
                Assert.That(_machine.IsInState(_start), Is.True);
            }

            _machine.LifeCycle(0.1f);

            Assert.That(_machine.IsInState(_next), Is.True);
        }

        [Test]
        public void TimerValueIsValidatedAgainWhenEnteringState()
        {
            AnimalVarId timerId = CreateTimer(_nextProbe.gameObject, _start);
            _context.State.Vars.Set(timerId, 1d);
            _machine.Initialize(_context, _start);
            _context.State.Vars.Remove(timerId);

            Assert.Throws<System.InvalidOperationException>(() => _machine.ForceSwitchState(_next));
        }

        private AnimalVarId CreateTimer(GameObject node, AnimalStateId nextState)
        {
            var timerId = ScriptableObject.CreateInstance<AnimalVarId>();
            _objects.Add(timerId);
            var transition = node.AddComponent<AnimalStateSwitchByVariableTimer>();
            SetField(transition, "_timerId", timerId);
            SetField(transition, "_nextState", nextState);
            return timerId;
        }

        [Test]
        public void ForcedTransitionStopsOldStateTicks()
        {
            _first.TickAction = () => _machine.ForceSwitchState(_next);
            _machine.Initialize(_context, _start);

            _machine.LifeCycle(0.02f);

            Assert.That(_machine.IsInState(_next), Is.True);
            Assert.That(_second.Ticks, Is.Zero);
            Assert.That(_second.Exits, Is.EqualTo(1));
            Assert.That(_nextProbe.Enters, Is.EqualTo(1));
            Assert.That(_nextProbe.Ticks, Is.Zero);
            _machine.LifeCycle(0.02f);
            Assert.That(_nextProbe.Ticks, Is.EqualTo(1));
        }

        [Test]
        public void LeavingAndReenteringStopsTheOriginalTick()
        {
            _first.TickAction = () =>
            {
                _machine.ForceSwitchState(_next);
                _machine.ForceSwitchState(_start);
            };
            _machine.Initialize(_context, _start);

            _machine.LifeCycle(0.02f);

            Assert.That(_machine.IsInState(_start), Is.True);
            Assert.That(_second.Ticks, Is.Zero);
            Assert.That(_second.Enters, Is.EqualTo(2));
            Assert.That(_second.Predictions, Is.EqualTo(1));
        }

        [Test]
        public void RequestingCurrentStateDoesNotInterruptTicks()
        {
            _first.TickAction = () => _machine.ForceSwitchState(_start);
            _machine.Initialize(_context, _start);

            _machine.LifeCycle(0.02f);

            Assert.That(_second.Ticks, Is.EqualTo(1));
            Assert.That(_second.Exits, Is.Zero);
            Assert.That(_second.Predictions, Is.EqualTo(1));
        }

        [Test]
        public void DisabledComponentIsExcludedFromStateLifecycle()
        {
            _first.enabled = false;
            _machine.Initialize(_context, _start);

            _machine.LifeCycle(0.02f);
            _machine.ForceSwitchState(_next);

            Assert.That(_first.Setups, Is.Zero);
            Assert.That(_first.Enters, Is.Zero);
            Assert.That(_first.Ticks, Is.Zero);
            Assert.That(_first.Exits, Is.Zero);
            Assert.That(_first.Predictions, Is.Zero);
            Assert.That(_second.Setups, Is.EqualTo(1));
            Assert.That(_second.Enters, Is.EqualTo(1));
            Assert.That(_second.Ticks, Is.EqualTo(1));
            Assert.That(_second.Exits, Is.EqualTo(1));
        }

        [Test]
        public void WallContactStopsMovementWithoutRestartingWait()
        {
            AnimalSetVarFlagObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;
            _context.State.Body.linearVelocity = new Vector3(2f, -1f, 0f);

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.True);
            Assert.That(_machine.IsInState(_start), Is.True);
            _machine.LifeCycle(0.02f);
            Assert.That(_machine.IsInState(_next), Is.True);
            Assert.That(_context.State.Body.linearVelocity, Is.EqualTo(new Vector3(0f, -1f, 0f)));
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.True);
            _machine.LifeCycle(0.02f);
            Assert.That(_nextProbe.Enters, Is.EqualTo(1));

            _machine.ForceSwitchState(_start);
            Assert.That(_context.State.Vars.TryGet(_contactFlag, out _), Is.False);
            _machine.LifeCycle(0.02f);
            Assert.That(_machine.IsInState(_start), Is.True);
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.True);
            _machine.LifeCycle(0.02f);
            Assert.That(_nextProbe.Enters, Is.EqualTo(2));
        }

        [Test]
        public void SnakeCanMoveAwayWhileStillTouchingWall()
        {
            AnimalSetVarFlagObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.left;

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_machine.IsInState(_start), Is.True);
        }

        [Test]
        public void GroundAndOtherAnimalsDoNotTriggerWallWait()
        {
            AnimalSetVarFlagObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;

            Assert.That(TouchWall(contact, wall, Vector3.up), Is.False);
            wall.gameObject.layer = 6;
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_machine.IsInState(_start), Is.True);
        }

        [Test]
        public void DeadAnimalIgnoresWallContacts()
        {
            AnimalSetVarFlagObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;
            _context.State.MarkDead();

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_nextProbe.Enters, Is.Zero);
        }

        [TestCase(0d)]
        [TestCase(1d)]
        [TestCase(-5d)]
        public void FlagPresenceRequestsTransitionRegardlessOfValue(double value)
        {
            CreateWallResponse(out _);
            _context.State.Vars.Set(_contactFlag, value);

            _machine.LifeCycle(0.02f);

            Assert.That(_machine.IsInState(_next), Is.True);
            Assert.That(_context.State.Vars.TryGet(_contactFlag, out _), Is.False);
        }

        private AnimalSetVarFlagObstacleContact CreateWallResponse(out Collider wall)
        {
            AnimalSetVarFlagObstacleContact contact = _machine.gameObject.AddComponent<AnimalSetVarFlagObstacleContact>();
            _contactFlag = ScriptableObject.CreateInstance<AnimalVarId>();
            _objects.Add(_contactFlag);
            SetField(contact, "_flagId", _contactFlag);
            AnimalStateSwitchByVarFlag predictor = _startObject.AddComponent<AnimalStateSwitchByVarFlag>();
            SetField(predictor, "_flagId", _contactFlag);
            SetField(predictor, "_nextState", _next);
            contact.SetupContext(_context);
            _nextProbe.gameObject.AddComponent<AnimalStateWait>();
            _machine.Initialize(_context, _start);
            GameObject obstacle = new("Wall");
            _objects.Add(obstacle);
            obstacle.layer = 8;
            wall = obstacle.AddComponent<BoxCollider>();
            return contact;
        }

        private static bool TouchWall(AnimalSetVarFlagObstacleContact contact, Collider wall, Vector3 normal)
        {
            return (bool)typeof(AnimalSetVarFlagObstacleContact)
                .GetMethod("TrySetFlag", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(contact, new object[] { wall, normal });
        }

        private GameObject CreateState(GameObject root, out AnimalStateId id)
        {
            id = ScriptableObject.CreateInstance<AnimalStateId>();
            _objects.Add(id);
            GameObject child = new("State");
            child.transform.SetParent(root.transform);
            SetField(child.AddComponent<AnimalStateNode>(), "_id", id);
            return child;
        }

        private static void SetField(object target, string name, object value)
        {
            target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        }
    }
}
