using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFEcommerceApp
{
    internal class MainItem
    {
        private readonly Type _ContentType;
        private readonly object _DataContext;

        private object _Content;
        public object Content
        {
            get
            {
                if(_Content == null)
                {
                    return CreateContent();
                }    
                return _Content;
            }
        }
        public string Name { get; private set; }
        public MainItem(string name, Type contentType, object dataContext = null)
        {
            Name = name;
            _ContentType = contentType;
            _DataContext = dataContext;
        }
        private object CreateContent()
        {
            var content = Activator.CreateInstance(_ContentType);
            if (_DataContext != null && content is FrameworkElement element)
            {
                element.DataContext = _DataContext;
            }

            return content;
        }
    }
}
