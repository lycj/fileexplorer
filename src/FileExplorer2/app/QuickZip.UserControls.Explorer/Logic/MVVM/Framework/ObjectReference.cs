/* Copyright (c) 2007, Dr. WPF
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   * Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 * 
 *   * The name Dr. WPF may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY Dr. WPF ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL Dr. WPF BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Reflection;

namespace QuickZip.Logic
{
    public class ObjectReference : MarkupExtension
    {
        public ObjectReference()
        {
        }

        public ObjectReference(object key)
        {
            _key = key;
        }

        public ObjectReference(object key, bool isDeclaration)
        {
            _key = key;
            _isDeclaration = isDeclaration;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object result = _key;

            // ensure that a key was specified
            if (_key == null)
            {
                throw new InvalidOperationException("The Key has not been specified for the ObjectReference.");
            }

            // determine whether this is a declaration or a reference
            bool isDeclaration = false;
            if (serviceProvider != null)
            {
                IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
                if (provideValueTarget != null)
                {
                    // treat the reference as a declaration if the IsDeclaration property is true
                    // or if its being set on the Declaration attached property
                    isDeclaration = _isDeclaration
                        || (provideValueTarget.TargetProperty == ObjectReference.DeclarationProperty);
                    if (isDeclaration)
                    {
                        References[_key] = new WeakReference(provideValueTarget.TargetObject);
                    }
                }
            }

            if (!isDeclaration)
            {
                WeakReference targetReference;
                if (References.TryGetValue(_key, out targetReference))
                {
                    result = targetReference.Target;
                }
            }

            return result;
        }

        #region Declaration Attached Property

        public static readonly DependencyProperty DeclarationProperty =
            DependencyProperty.RegisterAttached("Declaration", typeof(object), typeof(ObjectReference),
                new FrameworkPropertyMetadata(null));

        public static object GetDeclaration(DependencyObject d)
        {
            return (object)d.GetValue(DeclarationProperty);
        }

        public static void SetDeclaration(DependencyObject d, object value)
        {
            d.SetValue(DeclarationProperty, value);
        }

        #endregion

        [ThreadStatic]
        private static Dictionary<object, WeakReference> _references = null;

        private static Dictionary<object, WeakReference> References
        {
            get
            {
                if (_references == null)
                {
                    _references = new Dictionary<object, WeakReference>();
                }
                return _references;
            }
        }

        private object _key = null;
        public object Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private bool _isDeclaration = false;
        public bool IsDeclaration
        {
            get { return _isDeclaration; }
            set { _isDeclaration = value; }
        }
    }
}
