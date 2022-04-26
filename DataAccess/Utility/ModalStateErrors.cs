using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace DataAccess.Utility
{
    public class ModalStateErrors
    {
        public List<string> Errors { get; set; }

        public ModalStateErrors()
        {
            Errors = new List<string>();
        }

        public List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Errors.Add(error.ErrorMessage);
                }
            }
            return Errors;
        }

        internal object GetModelStateErrors(object modalState)
        {
            throw new NotImplementedException();
        }
    }
}