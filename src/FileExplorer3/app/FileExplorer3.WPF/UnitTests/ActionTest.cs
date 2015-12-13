//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;
//using FileExplorer.WPF.ViewModels;
//using FileExplorer.WPF.ViewModels.Actions;
//using Moq;
//using NUnit.Framework;

//namespace FileExplorer.WPF.UnitTests
//{
//    [TestFixture]
//    public class ActionTest
//    {

//        //public class TestEntryModel : EntryModelBase 
//        //{
//        //    public TestEntryModel(bool isDirectory, string label)
//        //    {                
//        //        this.IsDirectory = isDirectory;
//        //        this.Label = label;
//        //    }
//        //}

//        static Mock<IProfile> _profilevm;
//        static Mock<IEntryViewModel> _parentvm, _child1vm, _child2vm;
//        static List<IEntryModel> _entryList;
//        static List<IEntryViewModel> _entryVMList;

//        static Mock<IEntryViewModel> setupViewModel(IProfile profile, string label, bool isDirectory)
//        {
//            var entryModel = new Mock<IEntryModel>();

//            entryModel.Setup(em => em.Label).Returns(label);
//            entryModel.Setup(em => em.IsDirectory).Returns(isDirectory);
//            entryModel.Setup(em => em.Profile).Returns(profile);
//            var retVal = new Mock<IEntryViewModel>();
//            retVal.Setup(evm => evm.EntryModel).Returns(entryModel.Object);
            
//            return retVal;
//        }

//        static void setup()
//        {
//            _profilevm = new Mock<IProfile>();

//            _parentvm = setupViewModel(_profilevm.Object, "parent", true);
//            _child1vm = setupViewModel(_profilevm.Object, "child1", false);
//            _child2vm = setupViewModel(_profilevm.Object, "child2", false);

//            _entryList = new List<IEntryModel>() { _child1vm.Object.EntryModel, _child2vm.Object.EntryModel };
//            _entryVMList = new List<IEntryViewModel>() { _child1vm.Object, _child2vm.Object };
//            _profilevm.Setup(foo => foo.ListAsync(It.IsAny<IEntryModel>(), It.IsAny<Func<IEntryModel, bool>>())).ReturnsAsync(_entryList);
//        }



//        [Test]
//        public static void LoadEntryList_Test()
//        {
//            //Setup
//            setup();
//            var loadEL = new LoadEntryList(_parentvm.Object, null);
//            var context = new ActionExecutionContext();

//            //Action
//            ResultCompletionEventArgs args = loadEL.ExecuteAndWait(context);

//            //Assert
//            Assert.IsNull(args.Error);
//            Assert.IsNotNull(context["EntryList"]);
//            Assert.IsInstanceOf(typeof(IEnumerable<IEntryModel>), context["EntryList"]);
//            Assert.AreEqual(2, (context["EntryList"] as IEnumerable<IEntryModel>).Count());
//        }

//        [Test]
//        public static void AppendEntryList_Test()
//        {
//            //Setup
//            setup();
//            List<IEntryViewModel> _addedList = new List<IEntryViewModel>();
//            var _entryListvm = new Mock<IFileListViewModel>();
//            _entryListvm.Setup(elvm => elvm.Items.Add(It.IsAny<IEntryViewModel>()))
//                .Callback((IEntryViewModel elvm) => { _addedList.Add(elvm); });

//            var context = new ActionExecutionContext();
//            context["EntryList"] = _entryList;
//            //var appendEL = new AppendEntryList(_parentvm.Object, _entryListvm.Object);

//            ////Action
//            //ResultCompletionEventArgs args = appendEL.ExecuteAndWait(context);

//            ////Assert
//            //Assert.IsNull(args.Error);
//            //Assert.IsTrue(_addedList.Count == 2);
//        }

//        [Test]
//        public static void ToggleRename_Test()
//        {
//            setup();
//            Mock<IFileListViewModel> _selectable = new Mock<IFileListViewModel>();
//            _selectable.Setup(selvm => selvm.SelectedItems).Returns(_entryVMList);
//            var togRename = new ToggleRename(_selectable.Object);
//            var togRename2 = new ToggleRename(_selectable.Object);
//            var context = new ActionExecutionContext();
//            bool isEditing = false;
//            _child1vm.Setup(evm => evm.IsEditing).Returns(isEditing).Callback(() => isEditing = !isEditing);
//            _child2vm.Setup(evm => evm.IsEditing).Throws(new Exception("Child2.IsEditing invoked."));

//            //Action
//            ResultCompletionEventArgs args = togRename.ExecuteAndWait(context);
//            bool isEditing1 = isEditing; //false because not editable.

//            _child1vm.Setup(evm => evm.IsEditable).Returns(true);
//            ResultCompletionEventArgs args1 = togRename2.ExecuteAndWait(context);
//            bool isEditing2 = isEditing;

//            //Assert
//            Assert.IsNull(args.Error);
//            Assert.IsFalse(isEditing1);
//            Assert.IsTrue(isEditing2);
//            Assert.IsTrue(isEditing);
//        }


//        public interface IDummy : System.ComponentModel.INotifyPropertyChanged
//        {
//            int test { get; set; }
//            void add();
//        }
//        public static void WaitUntilPropertyChanged_Test()
//        {
            

//            //Setup
//            int test = 0;
//            var dummy = new Mock<IDummy>();
//            dummy.Setup(foo => foo.test).Returns(() => test);            
//            dummy.Setup(foo => foo.add()).Callback(() =>
//                {
//                    test++;
//                    dummy.Raise(foo => foo.PropertyChanged += null, 
//                        new PropertyChangedEventArgs("test"));
//                });
//            var wpc1 = new WaitTilPropertyChanged<int>(dummy.Object,() => dummy.Object.test);
//            test++;
//            var wpc2 = new WaitTilPropertyChanged<int>(dummy.Object, () => dummy.Object.test);
//            var context = new ActionExecutionContext();

//            Assert.AreEqual(0, wpc1.InitialValue);
//            Assert.AreEqual(1, wpc2.InitialValue);
//            Assert.AreEqual(1, test);
//            //Action
//            wpc1.ExecuteAndWait(context); //Return immediately.
//            wpc2.ExecuteAndWait(context, () => dummy.Object.add());

//            //Assert
//            Assert.AreEqual(1, wpc1.DestinationValue);
//            Assert.AreEqual(2, wpc2.DestinationValue);
//        }

//    }
//}
