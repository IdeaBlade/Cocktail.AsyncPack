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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class StaffingResource : AuditEntityBase
    {
        internal StaffingResource()
        {
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(MiddleName)
                           ? string.Format("{0} {1} {2}", FirstName.Trim(), MiddleName.Trim(), LastName.Trim())
                           : string.Format("{0} {1}", FirstName.Trim(), LastName.Trim());
            }
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the FirstName. </summary>
        [DataMember]
        [Required]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the MiddleName. </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>Gets or sets the LastName. </summary>
        [DataMember]
        [Required]
        public string LastName { get; set; }

        /// <summary>Gets or sets the Summary. </summary>
        [DataMember]
        [Required]
        public string Summary { get; set; }

        /// <summary>Gets the Addresses. </summary>
        [DataMember]
        public RelatedEntityList<Address> Addresses { get; internal set; }

        /// <summary>Gets the PhoneNumbers. </summary>
        [DataMember]
        public RelatedEntityList<PhoneNumber> PhoneNumbers { get; internal set; }

        /// <summary>Gets the Rates. </summary>
        [DataMember]
        public RelatedEntityList<Rate> Rates { get; internal set; }

        /// <summary>Gets the WorkExperience. </summary>
        [DataMember]
        public RelatedEntityList<WorkExperienceItem> WorkExperience { get; internal set; }

        /// <summary>Gets the Skills. </summary>
        [DataMember]
        public RelatedEntityList<Skill> Skills { get; internal set; }

        /// <summary>Gets or sets the PrimaryAddress. </summary>
        [NotMapped]
        public Address PrimaryAddress
        {
            get { return Addresses.FirstOrDefault(a => a.Primary); }
            set
            {
                if (value.StaffingResource != this)
                    throw new InvalidOperationException("Address is not associated with this StaffingResource.");

                Addresses.Where(a => a.Primary).ForEach(a => a.Primary = false);
                value.Primary = true;
            }
        }

        /// <summary>Gets or sets the PrimaryPhoneNumber. </summary>
        [NotMapped]
        public PhoneNumber PrimaryPhoneNumber
        {
            get { return PhoneNumbers.FirstOrDefault(a => a.Primary); }
            set
            {
                if (value.StaffingResource != this)
                    throw new InvalidOperationException("PhoneNumber is not associated with this StaffingResource.");

                PhoneNumbers.Where(p => p.Primary).ForEach(p => p.Primary = false);
                value.Primary = true;
            }
        }

        public override void Validate(VerifierResultCollection validationErrors)
        {
            if (Addresses.Count == 0)
                validationErrors.Add(new VerifierResult(false, "StaffingResource must have at least one address",
                                                                "Addresses"));

            if (PhoneNumbers.Count == 0)
                validationErrors.Add(new VerifierResult(false, "StaffingResource must have at least one phone number",
                                                                "PhoneNumbers"));
        }

        public static StaffingResource Create()
        {
            return new StaffingResource { Id = CombGuid.NewGuid() };
        }

        public Address AddAddress(AddressType type)
        {
            Address address = Address.Create(type);
            Addresses.Add(address);

            return address;
        }

        public void DeleteAddress(Address address)
        {
            address.EntityFacts.EntityAspect.Delete();
        }

        public PhoneNumber AddPhoneNumber(PhoneNumberType type)
        {
            PhoneNumber phoneNumber = PhoneNumber.Create(type);
            PhoneNumbers.Add(phoneNumber);

            return phoneNumber;
        }

        public void DeletePhoneNumber(PhoneNumber phoneNumber)
        {
            phoneNumber.EntityFacts.EntityAspect.Delete();
        }

        public Rate AddRate(RateType type)
        {
            Rate rate = Rate.Create(type);
            Rates.Add(rate);

            return rate;
        }

        public void DeleteRate(Rate rate)
        {
            rate.EntityFacts.EntityAspect.Delete();
        }

        public WorkExperienceItem AddWorkExperience()
        {
            WorkExperienceItem workExperienceItem = WorkExperienceItem.Create();
            WorkExperience.Add(workExperienceItem);

            return workExperienceItem;
        }

        public void DeleteWorkExperience(WorkExperienceItem workExperienceItem)
        {
            workExperienceItem.EntityFacts.EntityAspect.Delete();
        }

        public Skill AddSkill()
        {
            Skill skill = Skill.Create();
            Skills.Add(skill);

            return skill;
        }

        public void DeleteSkill(Skill skill)
        {
            skill.EntityFacts.EntityAspect.Delete();
        }

        [AfterSet("FirstName")]
        internal void AfterSetFirstName(object value)
        {
            EntityFacts.EntityAspect.ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }

        [AfterSet("MiddleName")]
        internal void AfterSetMiddleName(object value)
        {
            EntityFacts.EntityAspect.ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }

        [AfterSet("LastName")]
        internal void AfterSetLastName(object value)
        {
            EntityFacts.EntityAspect.ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }
    }
}