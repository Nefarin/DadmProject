using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public partial class ST_Segment : IModule
    {
        tempinput.setinputfilepath(@"c:\users\Ania\Desktop\DADM_Projekt\EKG.txt");
            tempinput.setoutputfilepath(@"c:\users\Ania\Desktop\DADM_Projekt\ekgqrsonsets.txt");
            uint fs = tempinput.getfrequency();
        vector<double> ecg = tempinput.getsignal();
        vector<double> dwt = listdwt(_ecg, 3, wavelet_type.db2)[1];
        vector<double> temp = vector<double>.build.dense(2);


        tempinput.setinputfilepath(@"c:\users\Ania\Desktop\DADM_Projekt\R_100.txt");

            list<int> rpeaks = new list<int>();
        vector<double> rpeaks = tempinput.getsignal();
            foreach (double singlepeak in rpeaks)
            {
                rpeaks.add((int)singlepeak);
            }


    waves_params param = new waves_params(wavelet_type.haar, 2, "analysis6");
    waves_data data = new waves_data(ecg, rpeaks, fs);


    waves testmodule = new waves();

    testmodule.init(param, data);
            testmodule.processdata();
            data = testmodule.data;

            vector<double> onsets = vector<double>.build.dense(data.qrsonsets.count);
            for (int i = 0; i<data.qrsonsets.count; i++)
            {
                onsets[i] = (double)data.qrsonsets[i];

            }

tempinput.writefile(360, onsets);
            vector<double> ends = vector<double>.build.dense(_qrsends.count);
            for (int i = 0; i<_qrsends.count; i++)
            {
                ends[i] = (double)_qrsends[i];

            }
            findp();
vector<double> ponset = vector<double>.build.dense(_ponsets.count);
            for (int i = 0; i<_ponsets.count; i++)
            {
                ponset[i] = (double)_ponsets[i];

            }
            vector<double> pends = vector<double>.build.dense(_pends.count);
            for (int i = 0; i<_pends.count; i++)
            {
                pends[i] = (double)_pends[i];

            }
            findt();
vector<double> tends = vector<double>.build.dense(_tends.count);
            for (int i = 0; i<_tends.count; i++)
            {
                tends[i] = (double)_tends[i];

            }

            tempinput.writefile(360, onsets);
            tempinput.setoutputfilepath(@"c:\users\phantom\desktop\dadm project\nowy folder\ekgqrsends.txt");
           
            console.read();
        }
    }
}

        }
    }
}
