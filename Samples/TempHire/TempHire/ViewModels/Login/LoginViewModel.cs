﻿//====================================================================================================================
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Cocktail;
using Common.Factories;
using DomainServices.Repositories;
using IdeaBlade.EntityModel;
using Security;

namespace TempHire.ViewModels.Login
{
    [Export]
    public class LoginViewModel : Screen
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPreLoader _preLoader;
        private readonly IWindowManager _windowManager;
        private string _failureMessage;
        private string _password;
        private string _username;
        private TaskCompletionSource<bool> _tcs;

        [ImportingConstructor]
        public LoginViewModel(IAuthenticationService authenticationService, IWindowManager windowManager,
                              [Import(AllowDefault = true)] IPreLoader preLoader)
        {
            Busy = new BusyWatcher();
            _authenticationService = authenticationService;
            _windowManager = windowManager;
            _preLoader = preLoader;
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "";
// ReSharper restore DoNotCallOverridableMethodsInConstructor

#if DEBUG
            _username = "Admin";
            _password = "password";
#endif
        }

        public IBusyWatcher Busy { get; private set; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
            set
            {
                _failureMessage = value;
                NotifyOfPropertyChange(() => FailureMessage);
                NotifyOfPropertyChange(() => FailureMessageVisible);
            }
        }

        public bool FailureMessageVisible
        {
            get { return !string.IsNullOrWhiteSpace(_failureMessage); }
        }

        public bool CanLogin
        {
            get { return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        public Task ShowAsync()
        {
            _tcs = new TaskCompletionSource<bool>();
            _windowManager.ShowDialog(this);
            return _tcs.Task;
        }

        public async void Login()
        {
            try
            {
                using (Busy.GetTicket())
                {
                    FailureMessage = "";

                    byte[] hash = CryptoHelper.GenerateKey(Password);
                    string password = Encoding.UTF8.GetString(hash, 0, hash.Length);
                    var credential = new LoginCredential(Username, password, null);
                    // Clear username and password fields
                    Username = null;
                    Password = null;

                    await _authenticationService.LoginAsync(credential);
                    if (_authenticationService.IsLoggedIn)
                    {
                        if (_preLoader != null)
                        {
                            try
                            {
                                await _preLoader.LoadAsync();
                            }
                            catch (Exception)
                            {
                                throw new Exception("Failed to preload lookup repository. Try again!");
                            }
                        }

                        TryClose();
                    }
                }
            }
            catch (Exception e)
            {
                FailureMessage = e.Message;
            }
        }

        public void KeyDown(KeyEventArgs args)
        {
            if (args.Key != Key.Enter)
                return;

            Login();
        }

        private void OnComplete()
        {
            _tcs.TrySetResult(true);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                OnComplete();
        }
    }

    [Export(typeof (IPartFactory<LoginViewModel>))]
    public class LoginViewModelFactory : PartFactoryBase<LoginViewModel>
    {
    }
}