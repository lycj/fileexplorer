using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;

namespace FileExplorer.WPF.UserControls
{
    //http://www.andyrace.com/2014/05/caliburn-micro-and-usercontrols.html
    public class FileExplorerBootStrapper : BootstrapperBase
    {
        private CompositionContainer container;

        /// <summary>
        /// The singleton bootstrapper
        /// </summary>
        private static BootstrapperBase bootstrapper;

        /// <summary>
        /// As assemblies don't have a well known entry point we need to initialise the bootstrapper on view construction AND view-model construction as we don't have control on how the user may access us
        /// </summary>
        internal static void Initialise()
        {
            if (null == bootstrapper)
            {
                bootstrapper = new FileExplorerBootStrapper();
            }
        }

        /// <summary>
        /// Call through to BootstrapperBase indicating that this isn't an application
        /// </summary>
        //[UsedImplicitly]
        public FileExplorerBootStrapper()
            : base(false)
        {
            this.Initialize();
        }

        /// <summary>Override this to include this usercontrol assembly</summary>
        /// <returns>Enumeration of 'Assembly's which include this assembly</returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var baseAssemblies = new List<Assembly>(base.SelectAssemblies());
            var thisAssembly = Assembly.GetAssembly(typeof(FileExplorerBootStrapper));
            if (!baseAssemblies.Contains(thisAssembly))
            {
                baseAssemblies.Add(thisAssembly);
            }

            // If this library is being accessed from a Caliburn enabled app then

            // this assembly may already be 'known' in the AssemblySource.Instance collection.
            // We need to remove these otherwise we'll get:
            //  "An item with the same key has already been added." (System.ArgumentException)
            // which (for my scenario) eventually manifested itself as a:
            //  "" (System.ComponentModel.Composition.CompositionException)
            foreach (var assembly in baseAssemblies.ToList().Where(newAssembly => AssemblySource.Instance.Contains(newAssembly)))
            {
                baseAssemblies.Remove(assembly);
            }


            return baseAssemblies;
        }

    }
}
