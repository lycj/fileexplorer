using FileExplorer.Defines;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FileExplorer.UIEventHub
{

    #region ShellDropHelper
    public abstract class ShellDropHelper : DropHelper, ISupportShellDrop
    {

        #region Constructor

        #endregion

        #region Methods

        public abstract IEnumerable<IDraggable> QueryDropDraggables(IDataObject da);
        public abstract DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffectsEx allowedEffects);

        public override DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            return Drop(draggables, null, allowedEffects);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion

    }
    #endregion

    #region NoDropHelper

    public class NoShellDropHelper : ShellDropHelper
    {
        public static ISupportShellDrop Instance = new NoShellDropHelper();

        public NoShellDropHelper()
            : base()
        {
            IsDroppable = false;
        }

        public override IEnumerable<IDraggable> QueryDropDraggables(IDataObject da)
        {
            yield break;
        }

        public override QueryDropEffects QueryDrop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            return QueryDropEffects.None;
        }

        public override DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffectsEx allowedEffects)
        {
            return DragDropEffectsEx.None;
        }
    }

    #endregion

    #region ShellDropHelper<T>
    public abstract class ShellDropHelper<M> : DropHelper<M>, ISupportShellDrop
        where M : class
    {

        #region Constructor


        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter">Convert from IDraggable to M</param>
        public ShellDropHelper(IValueConverter converter)
            : base(converter)
        {
        }


        #endregion

        #region Methods

        public abstract IEnumerable<M> QueryDropModels(IDataObject da);
        
        public virtual DragDropEffectsEx Drop(IEnumerable<M> models, IDataObject da, DragDropEffectsEx allowedEffects)
        {
            return Drop(models, allowedEffects);
        }

        public override DragDropEffectsEx Drop(IEnumerable<M> models, DragDropEffectsEx allowedEffects)
        {
            return Drop(models, null, allowedEffects);
        }


        public IEnumerable<IDraggable> QueryDropDraggables(IDataObject da)
        {
            return ConvertBack(QueryDropModels(da), true);
        }

        public virtual DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffectsEx allowedEffects)
        {
            return Drop(Convert(draggables, true), da, allowedEffects);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion
    }
    #endregion


    #region LambdaShellDropHelper

    public class LambdaShellDropHelper<M> : ShellDropHelper<M>
        where M : class
    {
        private Func<IEnumerable<M>, DragDropEffectsEx, QueryDropEffects> _queryDropFunc;
        private Func<IEnumerable<M>, IDataObject, DragDropEffectsEx, DragDropEffectsEx> _dropFunc;
        private IValueConverter _dataObjectConverter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter">Convert from IDraggable to M, you can use LambdaConverter or other IValueConverter.</param>
        /// /// <param name="dataObjectConverter">Convert from IEnumerable[M] to IDataObject, ConvertBack is used in this class, you can use LambdaConverter or other IValueConverter.</param>
        /// <param name="queryDropFunc">Called when QueryDrop, with models and allowEffects as parameter, return supported effect.</param>
        /// <param name="dropFunc">Call when Drop, with models, dataObject and allowedEffects as parameter, return actual effect.</param>
        public LambdaShellDropHelper(IValueConverter converter,
            IValueConverter dataObjectConverter,
            Func<IEnumerable<M>, DragDropEffectsEx, QueryDropEffects> queryDropFunc,
            Func<IEnumerable<M>, IDataObject, DragDropEffectsEx, DragDropEffectsEx> dropFunc)
            : base(converter)
        {
            _dataObjectConverter = dataObjectConverter;
            _queryDropFunc = queryDropFunc;
            _dropFunc = dropFunc;
        }

        public override IEnumerable<M> QueryDropModels(IDataObject da)
        {
            return _dataObjectConverter.ConvertBack(da, typeof(IEnumerable<M>), null, Thread.CurrentThread.CurrentUICulture)
                as IEnumerable<M>;
        }

        public override QueryDropEffects QueryDrop(IEnumerable<M> models, DragDropEffectsEx allowedEffects)
        {
            return _queryDropFunc(models, allowedEffects);
        }

        public override DragDropEffectsEx Drop(IEnumerable<M> models, IDataObject da, DragDropEffectsEx allowedEffects)
        {
            return _dropFunc(models, da, allowedEffects);
        }
    }

    #endregion
}
