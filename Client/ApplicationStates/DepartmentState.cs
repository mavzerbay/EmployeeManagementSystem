using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ApplicationStates
{
    public class DepartmentState
    {
        public Action? GeneralDepartmentAction { get; set; }
        public bool ShowGeneralDepartment { get; set; }

        public void GeneralDepartmentClicked()
        {
            ResetAllDepartments();
            ShowGeneralDepartment = true;
            GeneralDepartmentAction?.Invoke();
        }

        private void ResetAllDepartments()
        {
            ShowGeneralDepartment = false;
        }
    }
}