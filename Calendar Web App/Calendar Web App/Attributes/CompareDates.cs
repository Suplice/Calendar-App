using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CompareDatesAttribute : ValidationAttribute
	{
		private readonly string _StartDateProperty;
		private readonly string _EndDateProperty;
		public CompareDatesAttribute(string StartDate, string EndDate) {
			_StartDateProperty = StartDate;
			_EndDateProperty = EndDate;
		}

		protected override ValidationResult IsValid(object Value, ValidationContext Context)
		{
			var startDateProperty = Context.ObjectType.GetProperty(_StartDateProperty);
			var endDateProperty = Context.ObjectType.GetProperty(_EndDateProperty);

			if (startDateProperty == null || endDateProperty == null)
			{
				return new ValidationResult($"Property {_StartDateProperty} or {_EndDateProperty} cannot be null.");
			}

			var startDate = (DateTime)startDateProperty.GetValue(Context.ObjectInstance);
			var endDate = (DateTime)endDateProperty.GetValue(Context.ObjectInstance);


			if(startDate < endDate)
			{
				return ValidationResult.Success;
			}

			return new ValidationResult("End Date has to be later that Start Date");
		}	
	}
}
