using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTGUI
{
	// http://stackoverflow.com/a/31079674/1102322
	public static class PasswordBoxHelper
	{
		public static readonly DependencyProperty BoundPasswordProperty =
			DependencyProperty.RegisterAttached("BoundPassword",
				typeof(string),
				typeof(PasswordBoxHelper),
				new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

		public static string GetBoundPassword(DependencyObject d)
		{
			var box = d as PasswordBox;
			if (box != null)
			{
				// this funny little dance here ensures that we've hooked the
				// PasswordChanged event once, and only once.
				box.PasswordChanged -= PasswordChanged;
				box.PasswordChanged += PasswordChanged;
			}

			return (string)d.GetValue(BoundPasswordProperty);
		}

		public static void SetBoundPassword(DependencyObject d, string value)
		{
			if (string.Equals(value, GetBoundPassword(d)))
				return; // and this is how we prevent infinite recursion

			d.SetValue(BoundPasswordProperty, value);
		}

		private static void OnBoundPasswordChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var box = d as PasswordBox;

			if (box == null)
				return;

			box.Password = GetBoundPassword(d);
			SetSelection(box, box.Password.Length, 0);
			box.Focus();
		}

		private static void SetSelection(PasswordBox passwordBox, int start, int length)
		{
			passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
								 .Invoke(passwordBox, new object[] { start, length });
		} 

		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox password = sender as PasswordBox;

			SetBoundPassword(password, password.Password);
		}

	}
}
