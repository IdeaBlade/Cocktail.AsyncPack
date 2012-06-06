﻿// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using Cocktail;
using Common.Errors;
using Common.Toolbar;
using Common.Workspace;
using DomainModel;
using DomainServices.Repositories;
using DomainServices.SampleData;
using TempHire.Authentication;
using TempHire.ViewModels;
using TempHire.ViewModels.Login;
using TempHire.ViewModels.StaffingResource;

namespace TempHire.DesignTimeSupport
{
    public class ViewModelLocator : DesignTimeViewModelLocatorBase<TempHireEntities>
    {
        public StaffingResourceAddressListViewModel StaffingResourceAddressListViewModel
        {
            get
            {
                return (StaffingResourceAddressListViewModel)
                       new StaffingResourceAddressListViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                           null, DesignTimeErrorHandler.Instance,
                           DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSummaryViewModel StaffingResourceSummaryViewModel
        {
            get
            {
                return (StaffingResourceSummaryViewModel)
                       new StaffingResourceSummaryViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider), null,
                           DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceNameEditorViewModel StaffingResourceNameEditorViewModel
        {
            get
            {
                return new StaffingResourceNameEditorViewModel(
                    new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                    DesignTimeErrorHandler.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourcePhoneListViewModel StaffingResourcePhoneListViewModel
        {
            get
            {
                return (StaffingResourcePhoneListViewModel)
                       new StaffingResourcePhoneListViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                           null, DesignTimeErrorHandler.Instance,
                           DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceDetailViewModel StaffingResourceDetailViewModel
        {
            get
            {
                var rm = new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider);
                return new StaffingResourceDetailViewModel(
                    rm,
                    new StaffingResourceSummaryViewModel(rm, null, DesignTimeErrorHandler.Instance,
                                                         DesignTimeDialogManager.Instance),
                    new IStaffingResourceDetailSection[]
                        {
                            new StaffingResourceContactInfoViewModel(
                                new StaffingResourceAddressListViewModel(rm, null, DesignTimeErrorHandler.Instance,
                                                                         DesignTimeDialogManager.Instance),
                                new StaffingResourcePhoneListViewModel(rm, null, DesignTimeErrorHandler.Instance,
                                                                       DesignTimeDialogManager.Instance)),
                            new StaffingResourceRatesViewModel(rm, null, DesignTimeErrorHandler.Instance,
                                                               DesignTimeDialogManager.Instance)
                            ,
                            new StaffingResourceWorkExperienceViewModel(rm, DesignTimeErrorHandler.Instance),
                            new StaffingResourceSkillsViewModel(rm, DesignTimeErrorHandler.Instance)
                        },
                    DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceRatesViewModel StaffingResourceRatesViewModel
        {
            get
            {
                return (StaffingResourceRatesViewModel)
                       new StaffingResourceRatesViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                           null, DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceWorkExperienceViewModel StaffingResourceWorkExperienceViewModel
        {
            get
            {
                return (StaffingResourceWorkExperienceViewModel)
                       new StaffingResourceWorkExperienceViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSkillsViewModel StaffingResourceSkillsViewModel
        {
            get
            {
                return (StaffingResourceSkillsViewModel)
                       new StaffingResourceSkillsViewModel(
                           new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSearchViewModel StaffingResourceSearchViewModel
        {
            get
            {
                return new StaffingResourceSearchViewModel(new StaffingResourceSearchRepository(EntityManagerProvider),
                                                           DesignTimeErrorHandler.Instance).Start();
            }
        }

        public StaffingResourceManagementViewModel StaffingResourceManagementViewModel
        {
            get
            {
                var rm = new DesignTimeStaffingResourceUnitOfWorkManager(EntityManagerProvider);
                return
                    new StaffingResourceManagementViewModel(
                        new StaffingResourceSearchViewModel(new StaffingResourceSearchRepository(EntityManagerProvider),
                                                            DesignTimeErrorHandler.Instance), null, null, rm,
                        DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance, null);
            }
        }

        public ShellViewModel ShellViewModel
        {
            get
            {
                return
                    new ShellViewModel(new List<IWorkspace>(), new ToolbarManager(), new FakeAuthenticationService(),
                                       null, null).Start();
            }
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return new LoginViewModel(new FakeAuthenticationService(), null, null)
                           {FailureMessage = "FailureMessage at design time"};
            }
        }

        protected override IEntityManagerProvider<TempHireEntities> CreateEntityManagerProvider()
        {
            return new EntityManagerProvider<TempHireEntities>()
                .Configure(provider =>
                               {
                                   provider.WithConnectionOptions(ConnectionOptions.DesignTime.Name);
                                   provider.WithSampleDataProviders(new TempHireSampleDataProvider());
                               });
        }

        #region Nested type: DesignTimeDialogManager

        private class DesignTimeDialogManager : IDialogManager
        {
            private static IDialogManager _instance;

            public static IDialogManager Instance
            {
                get { return _instance ?? (_instance = new DesignTimeDialogManager()); }
            }

            #region IDialogManager Members

            public DialogOperationResult<T> ShowDialogAsync<T>(object content, IEnumerable<T> dialogButtons,
                                                               string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowDialogAsync<T>(object content, T defaultButton, T cancelButton,
                                                               IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<DialogResult> ShowDialogAsync(object content,
                                                                       IEnumerable<DialogResult> dialogButtons,
                                                                       string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowMessageAsync<T>(string message, IEnumerable<T> dialogButtons,
                                                                string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowMessageAsync<T>(string message, T defaultButton, T cancelButton,
                                                                IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<DialogResult> ShowMessageAsync(string message,
                                                                        IEnumerable<DialogResult> dialogButtons,
                                                                        string title = null)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Nested type: DesignTimeErrorHandler

        private class DesignTimeErrorHandler : IErrorHandler
        {
            private static IErrorHandler _instance;

            public static IErrorHandler Instance
            {
                get { return _instance ?? (_instance = new DesignTimeErrorHandler()); }
            }

            #region IErrorHandler Members

            public void HandleError(Exception ex)
            {
                // noop
            }

            #endregion
        }

        #endregion
    }
}