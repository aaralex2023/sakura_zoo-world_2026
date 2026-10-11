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
                _machine.IsInState, _machine.ForceSwitchState);
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
            AnimalSwitchOnObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;
            _context.State.Body.linearVelocity = new Vector3(2f, -1f, 0f);

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.True);
            Assert.That(_machine.IsInState(_next), Is.True);
            Assert.That(_context.State.Body.linearVelocity, Is.EqualTo(new Vector3(0f, -1f, 0f)));
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_nextProbe.Enters, Is.EqualTo(1));

            _machine.ForceSwitchState(_start);
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.True);
            Assert.That(_nextProbe.Enters, Is.EqualTo(2));
        }

        [Test]
        public void SnakeCanMoveAwayWhileStillTouchingWall()
        {
            AnimalSwitchOnObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.left;

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_machine.IsInState(_start), Is.True);
        }

        [Test]
        public void GroundAndOtherAnimalsDoNotTriggerWallWait()
        {
            AnimalSwitchOnObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;

            Assert.That(TouchWall(contact, wall, Vector3.up), Is.False);
            wall.gameObject.layer = 6;
            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_machine.IsInState(_start), Is.True);
        }

        [Test]
        public void DeadAnimalIgnoresWallContacts()
        {
            AnimalSwitchOnObstacleContact contact = CreateWallResponse(out Collider wall);
            _context.State.Heading = Vector3.right;
            _context.State.MarkDead();

            Assert.That(TouchWall(contact, wall, Vector3.left), Is.False);
            Assert.That(_nextProbe.Enters, Is.Zero);
        }

        private AnimalSwitchOnObstacleContact CreateWallResponse(out Collider wall)
        {
            AnimalSwitchOnObstacleContact contact = _machine.gameObject.AddComponent<AnimalSwitchOnObstacleContact>();
            SetField(contact, "_movementState", _start);
            SetField(contact, "_waitState", _next);
            contact.SetupContext(_context);
            _nextProbe.gameObject.AddComponent<AnimalStateWait>();
            _machine.Initialize(_context, _start);
            GameObject obstacle = new("Wall");
            _objects.Add(obstacle);
            obstacle.layer = 8;
            wall = obstacle.AddComponent<BoxCollider>();
            return contact;
        }

        private static bool TouchWall(AnimalSwitchOnObstacleContact contact, Collider wall, Vector3 normal)
        {
            return (bool)typeof(AnimalSwitchOnObstacleContact)
                .GetMethod("TryWait", BindingFlags.Instance | BindingFlags.NonPublic)
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
