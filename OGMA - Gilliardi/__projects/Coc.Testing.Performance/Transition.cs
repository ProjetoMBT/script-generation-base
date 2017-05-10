using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    /// <summary>
    /// Models the entity that results from parsing a transition within an Activity Diagram.
    /// </summary>
    [Serializable]
    public class Transition {

        [XmlAttribute("Source")]
        public string Source {get;set;}

        [XmlAttribute("Target")]
        public string Target {get;set;}

        //optimistic time (O)
        [XmlAttribute("OptimisticTime")]
        public double OptimisticTime {get;set;}

        //pessimistic time (P)
        [XmlAttribute("PessimisticTime")]
        public double PessimisticTime {get;set;}
        
        //most likely time (M)
        [XmlAttribute("MostLikeTime")]
        public double MostLikeTime {get;set;}

        //estimated time (E). PERT = OptimisticTime + (4 * MostLikeTime + PessimisticTime) / 6.
        [XmlAttribute("EstimatedTime")]
        public double EstimatedTime { get; set; }

        [XmlAttribute("Action")]
        public string Action {get;set;}

        [XmlAttribute("ThinkTime")]
        public double ThinkTime{get;set;}

        [XmlAttribute("Method")]
        public string Method {get;set;}

        [XmlAttribute("Probability")]
        public float Probability {get;set;}

        [XmlElement("Parameters")]
        public List<Parameter> Parameters {get;set;}

        [XmlElement("Cookies")]
        public List<Cookie> Cookies { get; set; }

        [XmlAttribute("Body")]
        public string Body { get; set; }

        [XmlAttribute("Referer")]
        public string Referer { get; set; }

        [XmlElement("SaveParameters")]
        public List<SaveParameter> SaveParameters { get; set; }


    }
}