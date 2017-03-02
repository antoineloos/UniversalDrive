using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneDriveSimpleSample.Utils
{
    public class VariableGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var viewModel = item as IResizable;

            if (null == viewModel)
            {
                base.PrepareContainerForItemOverride(element, item);
                return;
            }

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, viewModel.Width);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.Height);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
