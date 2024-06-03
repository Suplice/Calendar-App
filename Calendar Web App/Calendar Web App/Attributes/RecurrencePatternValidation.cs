using Calendar_Web_App.Data;
using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;

namespace Calendar_Web_App.Attributes
{
	public class RecurrencePatternValidationAttribute : ValidationAttribute
	{
		public string _startDatePropertyName { get;}
		public string _endDatePropertyName { get;}


		public RecurrencePatternValidationAttribute(string startDatePropertyName, string endDatePropertyName)
		{
			_startDatePropertyName = startDatePropertyName;
			_endDatePropertyName = endDatePropertyName;
		}


		public override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
			var endDateProperty = validationContext.ObjectType.GetProperty(_endDatePropertyName);

			if (startDateProperty == null || endDateProperty == null)
			{
				return new ValidationResult($"Unknown property: {startDateProperty} or {endDateProperty}");
			}


			var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);
			var endDate = (DateTime)endDateProperty.GetValue(validationContext.ObjectInstance);

			var duration = (endDate - startDate).TotalDays;

			
			
			if(value is RecurrencePattern recurrencePattern)
			{
				if(recurrencePattern == RecurrencePattern.daily && duration > 1)
				{
					return new ValidationResult("Events longer than a day cannot be set to recur daily");
				}
				else if(recurrencePattern == RecurrencePattern.weekly && duration > 7)
				{
					return new ValidationResult("Events longer than a week cannot be set to recur weekly");
				}
				else if(recurrencePattern == RecurrencePattern.monthly && duration > 31)
				{
					return new ValidationResult("Events longer than a month cannot be set to recur monthly");
				}
			}
			else
			{
				return new ValidationResult("Invalid recurrence pattern");
			}

			return ValidationResult.Success;
			

		}

	}
}
