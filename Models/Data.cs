using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NomiBotDS.Models
{
    public class Data
    {
        public string Version { get => version; set => version = value; }
        public string About { get => about; set => about = value; }

        string version = "⚙️ 1.0 beta";

        string about = "Привет! Я Герда, бот цель которого помогать администрации и игрокам\nСписок моих команд ты можешь получить командой ;help";


    }
}
