//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;

//namespace FileExplorer.WPF.ViewModels.Actions
//{
//    /// <summary>
//    /// Wait until value of the specified property changed (INotifyPropertyChanged.PropertyChanged), then call Completed
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class WaitTilPropertyChanged<T> : IResult        
//    {
//        #region Cosntructor

//        public WaitTilPropertyChanged(INotifyPropertyChanged parentModel,
//            Expression<Func<T>> propertyExpression)
//        {
//            _parentModel = parentModel;
//            _propertyExpression = propertyExpression;
//            _propertyName = GetName(propertyExpression);
//            InitialValue = GetProperty(propertyExpression);            
//        }

//        #endregion

//        #region Methods


//        //Alexandra Rusina - http://blogs.msdn.com/b/csharpfaq/archive/2010/03/11/how-can-i-get-objects-and-property-values-from-expression-trees.aspx
//        public static string GetName<T>(Expression<Func<T>> e)
//        {
//            var member = (MemberExpression)e.Body;
//            return member.Member.Name;
//        }
//        public static T GetProperty<T>(Expression<Func<T>> e)
//        {
//            var member = (MemberExpression)e.Body;
//            string propertyName = member.Member.Name;
//            T value = e.Compile()();
//            //Console.WriteLine("{0} : {1}", propertyName, value);
//            return value;
//        }

//        public event EventHandler<ResultCompletionEventArgs> Completed;

//        public void Execute(ActionExecutionContext context)
//        {
//            DestinationValue = GetProperty(_propertyExpression);
//            if (!(DestinationValue.Equals(InitialValue)))
//            {
//                Completed(this, new ResultCompletionEventArgs());
//            }
//            else
//                _parentModel.PropertyChanged += OnPropertyChanged;
//        }

//        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == _propertyName)
//            {
//                _parentModel.PropertyChanged -= OnPropertyChanged;
//                DestinationValue = GetProperty(_propertyExpression);
//                Completed(this, new ResultCompletionEventArgs());
//            }
//        }

//        #endregion

//        #region Data

//        INotifyPropertyChanged _parentModel;
//        string _propertyName;
//        Expression<Func<T>> _propertyExpression;

//        #endregion

//        #region Public Properties

//        public T InitialValue { get; private set; }
//        public T DestinationValue { get; private set; }

//        #endregion
//    }
//}
