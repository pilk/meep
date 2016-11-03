using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataCenter
{
    [XmlRoot("gameData")]
    public partial class DataTypes
    {
        [XmlElement("vfxtrigger")]
        public List<VFXDefinition> vfxList = new List<VFXDefinition>();

        [XmlElement("scene")]
        public List<SceneDefinition> scenesList = new List<SceneDefinition>();

        [XmlElement("character")]
        public List<CharacterDefinition> characterList = new List<CharacterDefinition>();

        [XmlElement("spawn")]
        public List<SpawnTriggerDefinition> spawnTriggerList = new List<SpawnTriggerDefinition>();
    };
}