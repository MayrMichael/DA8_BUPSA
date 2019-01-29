using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AGCut.ViewModels
{
    public class ScrollSyncBehavior : Behavior<TextBox>
    {
        public TextBox Second
        {
            get { return (TextBox)GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }

        public TextBox Third
        {
            get { return (TextBox)GetValue(thirdProperty); }
            set { SetValue(thirdProperty, value); }
        }

        public static readonly DependencyProperty SecondProperty =
          DependencyProperty.Register("Second", typeof(TextBox), typeof(ScrollSyncBehavior), new PropertyMetadata());

        public static readonly DependencyProperty thirdProperty =
          DependencyProperty.Register("Third", typeof(TextBox), typeof(ScrollSyncBehavior), new PropertyMetadata());

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            var scrollViewer = GetScrollViewer(AssociatedObject);
            scrollViewer.ScrollChanged -= OnScrollChanged;
            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs eventArgs)
        {

            var scrollViewer = GetScrollViewer(AssociatedObject);
            if(scrollViewer != null)
                scrollViewer.ScrollChanged += OnScrollChanged;
        }

        private ScrollViewer GetScrollViewer(DependencyObject dependencyObject)
        {
            try
            {
                var border = VisualTreeHelper.GetChild(dependencyObject, 0);
                return (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            }
            catch (Exception)
            {

                return null;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs eventArgs)
        {

            var scrollViewer = GetScrollViewer(Second);
            var scrollViewerThird = GetScrollViewer(Third);

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(eventArgs.VerticalOffset);
                scrollViewer.ScrollToHorizontalOffset(eventArgs.HorizontalOffset);

                scrollViewerThird.ScrollToVerticalOffset(eventArgs.VerticalOffset);
                scrollViewerThird.ScrollToHorizontalOffset(eventArgs.HorizontalOffset);
            }
        }
    }
}
