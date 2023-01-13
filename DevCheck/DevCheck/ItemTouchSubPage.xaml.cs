using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace DevCheck
{
    public sealed partial class ItemTouchSubPage : Page
    {
        Dictionary<uint, TrackedTouchPoint> pointers;

        PointerEventHandler pointerPressed;
        PointerEventHandler pointerMoved;
        PointerEventHandler pointerReleased;
        PointerEventHandler pointerExited;
        PointerEventHandler pointerEntered;
        PointerEventHandler pointerWheelChanged;

        public ItemTouchSubPage()
        {
            this.InitializeComponent();

            pointers = new Dictionary<uint, TrackedTouchPoint>();

            pointerPressed += new PointerEventHandler(Pointer_Pressed);
            pointerMoved += new PointerEventHandler(Pointer_Moved);
            pointerReleased += new PointerEventHandler(Pointer_Released);
            pointerExited += new PointerEventHandler(Pointer_Moved);
            pointerEntered += new PointerEventHandler(Pointer_Entered);
            pointerWheelChanged += new PointerEventHandler(Pointer_Wheel_Changed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TouchArea.PointerPressed += pointerPressed;
            TouchArea.PointerMoved += pointerMoved;
            TouchArea.PointerReleased += pointerReleased;
            TouchArea.PointerExited += pointerMoved;
            TouchArea.PointerEntered += pointerEntered;
            TouchArea.PointerWheelChanged += pointerWheelChanged;

            MainPage.Current.EnableStatusBar(false);
            MainPage.Current.EnableHeaderPanel(false);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            TouchArea.PointerPressed -= pointerPressed;
            TouchArea.PointerMoved -= pointerMoved;
            TouchArea.PointerReleased -= pointerReleased;
            TouchArea.PointerExited -= pointerMoved;
            TouchArea.PointerEntered -= pointerEntered;
            TouchArea.PointerWheelChanged -= pointerWheelChanged;

            MainPage.Current.EnableStatusBar(true);
            MainPage.Current.EnableHeaderPanel(true);

            base.OnNavigatingFrom(e);
        }

        void Pointer_Pressed(object sender, PointerRoutedEventArgs e)
        {
            TrackedTouchPoint trackedTouchPoint = new TrackedTouchPoint();
            trackedTouchPoint.pointerPoint = e.GetCurrentPoint(TouchArea);

            CreateOrUpdatePointer(trackedTouchPoint);
        }

        void Pointer_Entered(object sender, PointerRoutedEventArgs e)
        {
            TrackedTouchPoint trackedTouchPoint = new TrackedTouchPoint();
            trackedTouchPoint.pointerPoint = e.GetCurrentPoint(TouchArea);

            if (trackedTouchPoint.pointerPoint.IsInContact)
            {
                CreateOrUpdatePointer(trackedTouchPoint);
            }
        }

        void Pointer_Moved(object sender, PointerRoutedEventArgs e)
        {
            TrackedTouchPoint trackedTouchPoint = new TrackedTouchPoint();
            trackedTouchPoint.pointerPoint = e.GetCurrentPoint(TouchArea);

            if (trackedTouchPoint.pointerPoint.IsInContact)
            {
                CreateOrUpdatePointer(trackedTouchPoint);
            }
        }

        void Pointer_Released(object sender, PointerRoutedEventArgs e)
        {
            TrackedTouchPoint trackedTouchPoint = new TrackedTouchPoint();
            trackedTouchPoint.pointerPoint = e.GetCurrentPoint(TouchArea);

            HidePointer(trackedTouchPoint);
        }

        void Pointer_Wheel_Changed(object sender, PointerRoutedEventArgs e)
        {
            TrackedTouchPoint trackedTouchPoint = new TrackedTouchPoint();
            trackedTouchPoint.pointerPoint = e.GetCurrentPoint(TouchArea);

            if (trackedTouchPoint.pointerPoint.IsInContact)
            {
                CreateOrUpdatePointer(trackedTouchPoint);
            }
        }

        private void CreateOrUpdatePointer(TrackedTouchPoint trackedTouchPoint)
        {
            if (pointers.ContainsKey(trackedTouchPoint.pointerPoint.PointerId))
            {
                pointers[trackedTouchPoint.pointerPoint.PointerId].pointerPoint = trackedTouchPoint.pointerPoint;
            }
            else
            {
                pointers[trackedTouchPoint.pointerPoint.PointerId] = trackedTouchPoint;

                pointers[trackedTouchPoint.pointerPoint.PointerId].InsertPointer(TouchArea);
            }

            pointers[trackedTouchPoint.pointerPoint.PointerId].UpdatePointer(TouchArea);
        }

        private void HidePointer(TrackedTouchPoint trackedTouchPoint)
        {
            if (pointers.ContainsKey(trackedTouchPoint.pointerPoint.PointerId))
            {
                pointers[trackedTouchPoint.pointerPoint.PointerId].RemovePointer(TouchArea);

                pointers[trackedTouchPoint.pointerPoint.PointerId] = null;

                pointers.Remove(trackedTouchPoint.pointerPoint.PointerId);
            }
        }

        private void ExitTest()
        {
            MainPage.Current.NavigateToPage(typeof(ItemTouchPage));
        }
    }

    public class TrackedTouchPoint
    {
        private Border touchPointer { get; set; }
        public PointerPoint pointerPoint { get; set; }

        public TrackedTouchPoint()
        {
            touchPointer = new Border() { Height = 40, Width = 40, Background = new SolidColorBrush(Colors.Green), ManipulationMode = ManipulationModes.All };
        }

        public void InsertPointer(Canvas canvas)
        {
            canvas.Children.Add(touchPointer);
        }

        public void UpdatePointer(Canvas canvas)
        {
            TranslateTransform transform = new TranslateTransform();

            transform.X = pointerPoint.Position.X;
            transform.Y = pointerPoint.Position.Y;

            touchPointer.RenderTransform = transform;
        }

        public void RemovePointer(Canvas canvas)
        {
            canvas.Children.Remove(touchPointer);
        }
    }
}
