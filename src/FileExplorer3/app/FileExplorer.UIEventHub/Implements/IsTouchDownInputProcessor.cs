using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{

    public class IsTouchDownInputProcessor : InputProcessorBase
    {

        #region Constructors

        public IsTouchDownInputProcessor()
        {
            ProcessAllEvents = true;
        }

        #endregion

        #region Methods

        public override void Update(ref IUIInput input)
        {
            if (input.InputType == UIInputType.Touch && input.InputState != UIInputState.NotApplied)
                _touchState = input.InputState;

            input.Touch = _touchState;
        }

        #endregion

        #region Data

        private UIInputState _touchState = UIInputState.NotApplied;

        #endregion

        #region Public Properties


        #endregion

    }
}
