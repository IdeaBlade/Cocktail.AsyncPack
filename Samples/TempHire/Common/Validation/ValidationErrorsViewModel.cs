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

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Common.Factories;
using IdeaBlade.Validation;

namespace Common.Validation
{
    [Export]
    public class ValidationErrorsViewModel : Screen
    {
        private VerifierResultCollection _verifierResults;

        public VerifierResultCollection VerifierResults
        {
            get { return _verifierResults; }
            private set
            {
                _verifierResults = value;
                NotifyOfPropertyChange(() => VerifierResults);
            }
        }

        public ValidationErrorsViewModel Start(VerifierResultCollection verifierResults)
        {
            VerifierResults = verifierResults;
            return this;
        }
    }

    [Export(typeof(IPartFactory<ValidationErrorsViewModel>))]
    public class ValidationErrorsViewModelFactory : PartFactoryBase<ValidationErrorsViewModel>
    {
    }
}