//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Factories;
using Common.Toolbar;
using Common.Workspace;
using IdeaBlade.Core;
using Security.Messages;
using TempHire.ViewModels.Login;

namespace TempHire.ViewModels
{
    [Export]
    public class ShellViewModel : Conductor<IScreen>, IDiscoverableViewModel, IHandle<LoggedInMessage>,
                                  IHandle<LoggedOutMessage>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPartFactory<LoginViewModel> _loginFactory;
        private readonly IEnumerable<IWorkspace> _workspaces;
        private readonly IErrorHandler _errorHandler;
        private readonly NavigationService<IWorkspace> _navigationService;

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<IWorkspace> workspaces, IToolbarManager toolbar,
                              IAuthenticationService authenticationService, IPartFactory<LoginViewModel> loginFactory,
                              IErrorHandler errorHandler)
        {
            Toolbar = toolbar;
            _workspaces = workspaces;
            _authenticationService = authenticationService;
            _loginFactory = loginFactory;
            _errorHandler = errorHandler;
            _navigationService = new NavigationService<IWorkspace>(this)
                .Configure(config => config.WithActivator(navigation => ActivateItem(navigation.Target.Content)));
        }

        public IToolbarManager Toolbar { get; private set; }

        public bool IsLoggedIn
        {
            get { return _authenticationService.IsLoggedIn; }
        }

        #region IHandle<LoggedInMessage> Members

        public void Handle(LoggedInMessage message)
        {
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        #endregion

        #region IHandle<LoggedOutMessage> Members

        public void Handle(LoggedOutMessage message)
        {
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        #endregion

        public ShellViewModel Start()
        {
            var mainGroup = new ToolbarGroup(0);
            _workspaces.OrderBy(w => w.Sequence).ForEach(
                w => mainGroup.Add(new ToolbarAction(this, w.DisplayName, 
                    () => NavigateToWorkspace(w).HandleError(_errorHandler.HandleError))));

            var logoutGroup = new ToolbarGroup(100) { new ToolbarAction(this, "Logout", Logout) };

            Toolbar.Clear();
            Toolbar.AddGroup(mainGroup);
            Toolbar.AddGroup(logoutGroup);

            IWorkspace home = GetHomeScreen();
            if (home != null)
                NavigateToWorkspace(home).HandleError(_errorHandler.HandleError);

            return this;
        }

        public async Task Login()
        {
            await _loginFactory.CreatePart().ShowAsync();

#if !SILVERLIGHT
            if (!_authenticationService.IsLoggedIn)
                TryClose();
#endif
        }

        public async void Logout()
        {
            IWorkspace home = GetHomeScreen();
            LogFns.DebugWriteLineIf(home == null, "No workspace marked as default.");
            if (home == null) return;

            await NavigateToWorkspace(home);
            await _authenticationService.LogoutAsync();
            await Login();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();

#if !SILVERLIGHT
            DisplayName = "TempHire for WPF";
#endif
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // Launch login dialog
            Login().HandleError(_errorHandler.HandleError);
        }

        private IWorkspace GetHomeScreen()
        {
            return _workspaces.FirstOrDefault(w => w.IsDefault);
        }

        private async Task NavigateToWorkspace(IWorkspace workspace)
        {
            if (ActiveItem == workspace.Content)
                return;

            await _navigationService.NavigateToAsync(() => workspace);
        }
    }
}