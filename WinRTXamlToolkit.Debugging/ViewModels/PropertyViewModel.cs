using System;
using System.ComponentModel;
using System.Reflection;

namespace WinRTXamlToolkit.Debugging.ViewModels
{
    public class PropertyViewModel : BasePropertyViewModel
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyViewModel(DependencyObjectViewModel elementViewModel, PropertyInfo propertyInfo)
            : base(elementViewModel)
        {
            _propertyInfo = propertyInfo;
            this.Name = propertyInfo.Name;
        }

        public override object Value
        {
            get
            {
                object val;

                if (this.TryGetValue(this.ElementViewModel.Model, out val))
                {
                    return val;
                }

                return 0;
            }
            set
            {
                _propertyInfo.SetValue(this.ElementViewModel.Model, value);
                _isDefault = null;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CanResetValue));
                this.OnPropertyChanged(nameof(this.IsDefault));
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _propertyInfo.PropertyType;
            }
        }

        public override string Category
        {
            get
            {
                return MiscCategoryName;
            }
        }

        public override bool IsDefault
        {
            get
            {
                if (_isDefault == null)
                {
                    var defaultValueAttribute =
                        _propertyInfo.GetCustomAttribute(typeof (DefaultValueAttribute)) as DefaultValueAttribute;

                    if (defaultValueAttribute != null)
                    {
                        _isDefault = this.Value == defaultValueAttribute.Value;
                    }
                    else
                    {
                        _isDefault = false;
                    }
                }

                return _isDefault.Value;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                var sm = _propertyInfo.SetMethod;

                return sm == null;
            }
        }

        public override bool CanResetValue
        {
            get
            {
                return false;
            }
        }

        public override void ResetValue()
        {
            _isDefault = null;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.CanResetValue));
            this.OnPropertyChanged(nameof(this.IsDefault));
        }

        public override bool CanAnalyze
        {
            get
            {
                return false;
            }
        }

        public override void Analyze()
        {
        }

        #region TryGetValue()
        public override bool TryGetValue(object model, out object val)
        {
            val = _propertyInfo.GetValue(model, new object[] { });
            return true;
        } 
        #endregion
    }
}