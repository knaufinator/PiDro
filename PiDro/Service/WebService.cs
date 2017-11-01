using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pidro.Service
{
    public class WebService: IWebService
    {
        public WebService()
        {
            //pass in access to sensor tiles ?
        }

        public object GetData()
        {
            //using sensor tiles, get data from each, send out list of data with names so android app can decipher what each data point is.
            return null;
        }
    }
}
