// ====================================================================================================================
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
using System.Linq;
using System.Threading.Tasks;
using Cocktail;
using Cocktail.Contrib.UnitOfWork;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Factories
{
    public class StaffingResourceFactory : IFactory<StaffingResource>
    {
        private readonly IRepository<AddressType> _addressTypes;
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;
        private readonly IRepository<PhoneNumberType> _phoneNumberTypes;

        public StaffingResourceFactory(IEntityManagerProvider<TempHireEntities> entityManagerProvider,
                                       IRepository<AddressType> addressTypes,
                                       IRepository<PhoneNumberType> phoneNumberTypes)
        {
            _entityManagerProvider = entityManagerProvider;
            _addressTypes = addressTypes;
            _phoneNumberTypes = phoneNumberTypes;
        }

        private TempHireEntities EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IFactory<StaffingResource> Members

        public OperationResult<StaffingResource> CreateAsync(Action<StaffingResource> onSuccess = null,
                                                             Action<Exception> onFail = null)
        {
            return CreateAsyncCore().OnComplete(onSuccess, onFail);
        }

        #endregion

        private async Task<StaffingResource> CreateAsyncCore()
        {
            var staffingResource = StaffingResource.Create();
            EntityManager.AddEntity(staffingResource);

            var addressType = (await _addressTypes.FindAsync(t => t.Default)).First();
            staffingResource.AddAddress(addressType);
            staffingResource.PrimaryAddress = staffingResource.Addresses.First();

            var phoneType = (await _phoneNumberTypes.FindAsync(t => t.Default)).First();
            staffingResource.AddPhoneNumber(phoneType);
            staffingResource.PrimaryPhoneNumber = staffingResource.PhoneNumbers.First();

            return staffingResource;
        }
    }
}