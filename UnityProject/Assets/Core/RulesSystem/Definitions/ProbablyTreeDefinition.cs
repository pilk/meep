using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DataCenter
{
    abstract public class ProbabilityTreeDefinition : DataObjectTemplate
    {
        public class Result : DataObjectTemplate
        {
            [XmlAttribute]
            public string key { get { return _key; } set { _key = value; this.m_changeList.Add("key"); } }
            private string _key = "";

            [XmlAttribute]
            public string value { get { return _value; } set { _value = value; this.m_changeList.Add("value"); } }
            private string _value = "";
        };

        public class Node : DataObjectTemplate
        {
            public enum Operation { FIXED, RANDOM };

            [XmlAttribute]
            public Operation operation { get { return _operation; } set { _operation = value; this.m_changeList.Add("operation"); } }
            private Operation _operation = Operation.FIXED;

            [XmlAttribute]
            public int count { get { return _count; } set { _count = value; this.m_changeList.Add("count"); } }
            private int _count = 1;

            [XmlAttribute]
            public int weight { get { return _weight; } set { _weight = value; this.m_changeList.Add("weight"); } }
            private int _weight = 1;

            [XmlElement("node")]
            public List<Node> nodeList = new List<Node>();

            [XmlElement("result")]
            public List<Result> results = new List<Result>();

            public void Execute(ref List<Result> results, System.Random random)
            {
                for (int loopCount = 0; loopCount < this.count; loopCount++)
                {
                    results.AddRange(this.results);

                    int nodeListCount = nodeList.Count;
                    if (nodeListCount == 0)
                        return;

                    switch (operation)
                    {
                        case Operation.FIXED:
                            for (int i = 0; i < nodeListCount; ++i)
                            {
                                nodeList[i].Execute(ref results, random);
                            }
                            break;
                        case Operation.RANDOM:
                            int totalWeight = 0;
                            for (int i = 0; i < nodeListCount; ++i)
                                totalWeight += nodeList[i].weight;
                            int randomWeight = random.Next(totalWeight);
                            int accumulatedWeight = 0;
                            for (int i = 0; i < nodeListCount; ++i)
                            {
                                accumulatedWeight += nodeList[i].weight;
                                if (accumulatedWeight >= randomWeight)
                                {
                                    nodeList[i].Execute(ref results, random);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        };

        [XmlElement("node")]
        public Node root;

        [XmlElement]
        public int random { get { return _random; } set { _random = value; this.m_changeList.Add("random"); } }
        protected int _random = 1;


        private System.Random m_random = null;


        public List<Result> Execute()
        {
            if (m_random == null)
            {
                m_random = (random != 0) ? (new Random(random)) : (new Random());
            }

            List<Result> results = new List<Result>();
            root.Execute(ref results, m_random);
            return results;
        }
    };
}
