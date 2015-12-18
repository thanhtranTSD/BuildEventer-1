/*
<License>
Copyright 2015 Virtium Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http ://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</License>
 */

using System;
using System.Reflection;

namespace PostBuild
{
    public static class AssemblyInfoExtent
    {
        #region Public Functions
        public static string Title
        {
            get
            {
                AssemblyTitleAttribute attribute = GetAssemblyAttribute<AssemblyTitleAttribute>();
                if (null != attribute)
                {
                    return attribute.Title;
                }
                return string.Empty;
            }
        }

        public static string Company
        {
            get
            {
                AssemblyCompanyAttribute attribute = GetAssemblyAttribute<AssemblyCompanyAttribute>();
                if (null != attribute)
                {
                    return attribute.Company;
                }
                return string.Empty;
            }
        }

        public static string Copyright
        {
            get
            {
                AssemblyCopyrightAttribute attribute = GetAssemblyAttribute<AssemblyCopyrightAttribute>();
                if (null != attribute)
                {
                    return attribute.Copyright;
                }
                return string.Empty;
            }
        }

        public static string Version
        {
            get
            {
                AssemblyFileVersionAttribute attribute = GetAssemblyAttribute<AssemblyFileVersionAttribute>();
                if (null != attribute)
                {
                    return attribute.Version;
                }
                return string.Empty;
            }
        }
        #endregion

        #region Private Function
        private static T GetAssemblyAttribute<T>() where T : Attribute
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), true);
            if ((null == attributes) || (0 == attributes.Length))
            {
                return null;
            }
            return (T)attributes[0];
        }
        #endregion
    }
}
