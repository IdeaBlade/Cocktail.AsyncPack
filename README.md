Cocktail.AsyncPack
==================

The "Cocktail Async Pack for Visual Studio 2012" enables Cocktail/DevForce projects targeting .NET Framework 4.0 or Silverlight 5 to use the Async language feature in C# 5 and Visual Basic 11. This pack requires Visual Studio 2012 and will not work with Visual Studio 2010.

## Usage and Availability##

The pack is available via NuGet. The NuGet package automatically downloads and installs the required version of Cocktail as well as the “Microsoft Async Targeting Pack for Visual Studio 2012”, which is required in order to use the Async language feature in .NET Framework 4.0 and Silverlight 5.

The NuGet package is available at the following location:
 
[http://www.nuget.org/packages/Cocktail.AsyncPack](http://www.nuget.org/packages/Cocktail.AsyncPack)
 
With the “Cocktail Async Pack” added to a project, all Cocktail and DevForce asynchronous operations become awaitable as in the following example.

    [Export]
    public class MainPageViewModel : Screen
    {
        private IDialogManager _dialogManager;
        private NorthwindIBEntities _manager;
 
        [ImportingConstructor]
        public MainPageViewModel(IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
            _manager = new NorthwindIBEntities();
        }
 
        public async void SampleAction()
        {
            var customers = await _manager.Customers.Where(c => c.Country == "UK").ExecuteAsync();
 
            await _dialogManager.ShowMessageAsync(string.Format("Found {0} customers.", 
                                                  customers.Count()), DialogButtons.Ok);
 
            var employees = await _manager.Employees.ExecuteAsync();
 
            await _dialogManager.ShowMessageAsync(string.Format("Found {0} employees.", 
                                                  employees.Count()), DialogButtons.Ok);
        }
    }