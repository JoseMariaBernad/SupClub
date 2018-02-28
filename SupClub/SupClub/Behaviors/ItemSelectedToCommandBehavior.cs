using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SupClub.Behaviors
{
    public class ItemSelectedToCommandBehavior : Behavior<ListView>
    {
        public static readonly BindableProperty CommandProperty = 
            BindableProperty.Create(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(ItemSelectedToCommandBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.ItemSelected += ListView_ItemSelected;
            bindable.BindingContextChanged += ListView_BindingContextChanged;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.ItemSelected -= ListView_ItemSelected;
            bindable.BindingContextChanged -= ListView_BindingContextChanged;
        }

        private void ListView_BindingContextChanged(object sender, EventArgs eventArgs)
        {
            var listView = sender as ListView;
            BindingContext = listView?.BindingContext;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Command.Execute(null);
            //var listView = sender as ListView;
            //var vm = listView.BindingContext as MainViewModel;
            //vm.SelectCommand.Execute(null);
        }
    }
}
