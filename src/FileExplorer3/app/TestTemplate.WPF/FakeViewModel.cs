using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTemplate.WPF
{
    public class FakeViewModel : INotifyPropertyChanged
    {

        private static void generate(FakeViewModel root, int level, string str = "")
        {
            if (level > 0)
                for (int i = 1; i < 5; i++)
                {
                    var vm = new FakeViewModel()
                    {
                        Header = "Sub" + str + i.ToString(),
                        Value = (root.Value + "\\Sub" + str + i.ToString()).TrimStart('\\'),
                        Latency = root.Latency, 
                        Parent = root
                    };
                    generate(vm, level - 1, str + i.ToString());
                    root._subDirectories.Add(vm);
                }
        }

        public static FakeViewModel GenerateFakeViewModels(TimeSpan latency)
        {
            var root = new FakeViewModel() { Latency = latency };
            generate(root, 5);
            return root;
        }

        public FakeViewModel(params FakeViewModel[] rootModels)
        {
            Header = "Root";
            Value = "";
            Latency = TimeSpan.FromSeconds(0);
            _subDirectories = new ObservableCollection<FakeViewModel>();
            foreach (var rm in rootModels)
            {
                rm.Parent = this;
                _subDirectories.Add(rm);
            }
        }

        public FakeViewModel(string header, params string[] subHeaders)
        {
            Header = header;
            Value = header;
            SubDirectories = new ObservableCollection<FakeViewModel>();
            foreach (var sh in subHeaders)
                _subDirectories.Add(new FakeViewModel(sh) { Value = header + "\\" + sh, Parent = this });
        }

        public override string ToString()
        {
            return Value;
        }

        private ObservableCollection<FakeViewModel> _subDirectories;
        public FakeViewModel Parent { get; set; }
        public string Header { get; set; }
        public string Value { get; set; }
        public TimeSpan Latency { get; set; }
        bool _loaded = false;
        public ObservableCollection<FakeViewModel> SubDirectories
        {
            get
            {
                if (!_loaded)
                {
                    _loaded = true;
                    Thread.Sleep(Latency);
                }
                return _subDirectories;
            }
            set { _subDirectories = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
