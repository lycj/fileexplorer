using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuickZip.UserControls
{

     public abstract class DialogEventArgs : RoutedEventArgs
     {
         protected DialogEventArgs(RoutedEvent routedEvent, string title, MessageBoxButton buttons) 
            :base(routedEvent) 
         {             
             Title = title;
             DialogButtons = buttons;
         }

         public Type InterfaceType { get; protected set; }
         public object DialogContent { get; protected set; }
         public MessageBoxButton DialogButtons { get; protected set; }
         public MessageBoxResult DialogResult { get; set; }
         public string Title { get; private set; }         
     }
    public class DialogEventArgs<I> : DialogEventArgs where I: class 
    {
        public DialogEventArgs(RoutedEvent routedEvent, string title, MessageBoxButton buttons) 
            :base(routedEvent, title, buttons)
        {
            InterfaceType = typeof(I);
            DialogContent = DialogControl.RetrieveItem<I>();
        }

        public DialogEventArgs(string title, MessageBoxButton buttons)
            : this(DialogControl.ShowDialogEvent, title, buttons)
        {
        }

        public DialogEventArgs()
            : this("", MessageBoxButton.OK)
        {            
        }

        public I DialogInterface { get { return DialogContent as I; } }
        
        
    }

    //public class DialogEventArgs<T> : DialogEventArgs
    //{
    //    public DialogEventArgs(T prop) : base(typeof(T)) { Properties = prop; }
    //    public T Properties { get; private set; }
    //}

    public delegate void DialogEventHandler(object sender, DialogEventArgs eventArgs);

    public abstract class DialogAdornerFactory
    {
        public Type ImlementedType { get; protected set; }
    }

    public abstract class DialogAdornerFactory<I> : DialogAdornerFactory
        where I : class
    {
        public DialogAdornerFactory()
        {
            ImlementedType = typeof(I);
        }

        public abstract I CreateAdornerItem();
    }

    public class DialogAdornerFactory<A, I> : DialogAdornerFactory<I>
        where I : class
        where A : I
    {
        public override I CreateAdornerItem()
        {
            return Activator.CreateInstance<A>();
        }
    }


    public class DialogControl : ContentControl
    {
        #region Events        
        public static readonly RoutedEvent ShowDialogEvent = EventManager.RegisterRoutedEvent("ShowDialog", RoutingStrategy.Bubble,
            typeof(DialogEventHandler), typeof(DialogControl));

        #endregion

        #region Constructor
        static DialogControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogControl),
                new FrameworkPropertyMetadata(typeof(DialogControl)));
        }

        public DialogControl()
        {
            this.AddHandler(ShowDialogEvent, new DialogEventHandler(OnShowDialog));
        }
        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                new ExecutedRoutedEventHandler(delegate { IsDialogVisible = false; })));
        }

        public void OnShowDialog(object s, DialogEventArgs e)
        {            
            IsDialogVisible = true;
            e.Handled = true;
            DialogContent = e.DialogContent;
            DialogButtons = e.DialogButtons;
            //DialogTitle = e.Title;                       
        }

        #region Factory resolve related

        static Dictionary<Type, DialogAdornerFactory> _factoryList = new Dictionary<Type, DialogAdornerFactory>();

        public static void RegisterFactory<I>(DialogAdornerFactory factory)
        {
            _factoryList.Add(typeof(I), factory);
        }

        public static void RegisterFactory<A, I>(DialogAdornerFactory<A, I> factory)
            where I : class
            where A : Control, I
        {
            RegisterFactory<I>(factory);
        }

        public static void RegisterFactory<A, I>()
            where I : class
            where A : Control, I
        {
            RegisterFactory(new DialogAdornerFactory<A, I>());
        }

        public static DialogAdornerFactory<I> RetrieveFactory<I>()
            where I : class
        {
            return _factoryList.ContainsKey(typeof(I)) ?
                (DialogAdornerFactory<I>)_factoryList[typeof(I)] : null;
        }

        public static I RetrieveItem<I>()
           where I : class
        {
            return RetrieveFactory<I>().CreateAdornerItem();
        }    

        #endregion

        #endregion



        #region Public Properties

        public object DialogContent
        {
            get { return (object)GetValue(DialogContentProperty); }
            set { SetValue(DialogContentProperty, value); }
        }        
        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register("DialogContent", typeof(object), typeof(DialogControl), new UIPropertyMetadata(null));



        public MessageBoxButton DialogButtons
        {
            get { return (MessageBoxButton)GetValue(DialogButtonsProperty); }
            set { SetValue(DialogButtonsProperty, value); }
        }
        
        public static readonly DependencyProperty DialogButtonsProperty =
            DependencyProperty.Register("DialogButtons", typeof(MessageBoxButton), 
            typeof(DialogControl), new UIPropertyMetadata(MessageBoxButton.OK));
       

        public object DialogTitle
        {
            get { return (object)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }
        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(object), typeof(DialogControl), new UIPropertyMetadata("Title"));

        public bool IsDialogVisible
        {
            get { return (bool)GetValue(IsDialogVisibleProperty); }
            set { SetValue(IsDialogVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsDialogVisibleProperty =
            DependencyProperty.Register("IsDialogVisible",
            typeof(bool), typeof(DialogControl), new UIPropertyMetadata(false));

        #endregion




    }
}
