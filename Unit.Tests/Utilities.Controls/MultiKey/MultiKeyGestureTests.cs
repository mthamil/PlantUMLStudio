using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.Controls.MultiKey;
using Xunit;

namespace Unit.Tests.Utilities.Controls.MultiKey
{
	public class MultiKeyGestureTests : IDisposable
	{
		[Fact]
		public void Test_Matches_IncorrectStartModifier()
		{
			// Arrange.
			PressKeys(Key.LeftShift, Key.V);
			var args = CreateKeyEventArgs(Key.V);

			// Act.
			bool matches = gesture.Matches(null, args);

			// Assert.
			Assert.False(matches);
		}

		[Fact]
		public void Test_Matches_IncorrectStartKey()
		{
			// Arrange.
			PressKeys(Key.LeftCtrl, Key.X);
			var args = CreateKeyEventArgs(Key.X);

			// Act.
			bool matches = gesture.Matches(null, args);

			// Assert.
			Assert.False(matches);
		}

		[Fact]
		public void Test_Matches_CorrectStartSequence_TimedOut()
		{
			// Arrange.
			MultiKeyGesture.MaximumDelayBetweenKeyPresses = TimeSpan.FromMilliseconds(100);

			PressKeys(Key.LeftCtrl, Key.V);

			var args = CreateKeyEventArgs(Key.V);

			// Act.
			bool firstMatch = gesture.Matches(null, args);

			// Assert.
			Assert.False(firstMatch);

			// Arrange.
			Thread.Sleep(105);

			PressKeys(Key.LeftAlt, Key.A);
			args = CreateKeyEventArgs(Key.A);

			// Act.
			bool secondMatch = gesture.Matches(null, args);

			// Assert.
			Assert.False(secondMatch);
		}

		[Fact]
		public void Test_Matches_IncorrectNextModifier()
		{
			// Arrange.
			MultiKeyGesture.MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(5);

			PressKeys(Key.LeftCtrl, Key.V);

			var args = CreateKeyEventArgs(Key.V);
			gesture.Matches(null, args);

			PressKeys(Key.LeftCtrl, Key.A);
			args = CreateKeyEventArgs(Key.A);

			// Act.
			bool matches = gesture.Matches(null, args);

			// Assert.
			Assert.False(matches);
		}

		[Fact]
		public void Test_Matches_IncorrectNextKey()
		{
			// Arrange.
			MultiKeyGesture.MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(5);

			PressKeys(Key.LeftCtrl, Key.V);

			var args = CreateKeyEventArgs(Key.V);
			gesture.Matches(null, args);

			PressKeys(Key.LeftAlt, Key.X);
			args = CreateKeyEventArgs(Key.X);

			// Act.
			bool matches = gesture.Matches(null, args);

			// Assert.
			Assert.False(matches);
		}

		[Fact]
		public void Test_Matches()
		{
			// Arrange.
			MultiKeyGesture.MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(5);

			PressKeys(Key.LeftCtrl, Key.V);

			var args = CreateKeyEventArgs(Key.V);
			gesture.Matches(null, args);

			PressKeys(Key.LeftAlt, Key.A);
			args = CreateKeyEventArgs(Key.A);

			// Act.
			bool matches = gesture.Matches(null, args);

			// Assert.
			Assert.True(matches);
		}

		private KeyEventArgs CreateKeyEventArgs(Key key)
		{
			return new KeyEventArgs(keyboardDevice, new PresentationSourceStub(), 0, key)
			{
				RoutedEvent = UIElement.KeyDownEvent
			};
		}

		private void PressKeys(params Key[] keys)
		{
			keyboardDevice.PressedKeys.Clear();
			foreach (var key in keys)
			{
				keyboardDevice.PressedKeys.Add(key);
			}
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			MultiKeyGesture.MaximumDelayBetweenKeyPresses = originalTimeout;
		}

		#endregion

		private readonly MultiKeyGesture gesture = new MultiKeyGesture(new List<KeyInput> 
		{ 
			new KeyInput { Modifier = ModifierKeys.Control, Key = Key.V },
			new KeyInput { Modifier =  ModifierKeys.Alt, Key = Key.A }
		});

		private readonly KeyboardDeviceStub keyboardDevice = new KeyboardDeviceStub();
		private readonly TimeSpan originalTimeout = MultiKeyGesture.MaximumDelayBetweenKeyPresses;

		public class KeyboardDeviceStub : KeyboardDevice
		{
			public KeyboardDeviceStub() 
				: base(InputManager.Current) 
			{
				PressedKeys = new HashSet<Key>();
			}

			#region Overrides of KeyboardDevice

			protected override KeyStates GetKeyStatesFromSystem(Key key)
			{
				return PressedKeys.Contains(key) ? KeyStates.Down : KeyStates.None;
			}

			#endregion

			public ISet<Key> PressedKeys { get; private set; }
		}

		public class PresentationSourceStub : PresentationSource
		{
			#region Overrides of PresentationSource

			protected override CompositionTarget GetCompositionTargetCore()
			{
				return null;
			}

			public override Visual RootVisual { get; set; }

			public override bool IsDisposed
			{
				get { return false; }
			}

			#endregion
		}
	}
}