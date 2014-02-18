using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Linq;

public static class MyExtensions
{
	public static Stream ToStream(this string str)
	{
		MemoryStream stream = new MemoryStream();
		StreamWriter writer = new StreamWriter(stream);
		writer.Write(str);
		writer.Flush();
		stream.Position = 0;
		return stream;
	}
}

public class XML_Loader{

//XML subs

    //getting values
	public static string getStr(XmlNode element,string name){
		if (element[name]==null) return "";
		return element[name].InnerText;
	}
	
	public static int getInt(XmlNode element,string name){
		if (element[name]==null) return 0;
		return int.Parse(element[name].InnerText);
	}
	
	public static float getFlt(XmlNode element,string name){
		if (element[name]==null) return 0f;
		return float.Parse(element[name].InnerText);
	}
	
	public static string getAttStr(XmlNode element,string name){
		if (element.Attributes[name]==null) return "";
		return element.Attributes[name].Value;
	}
	
	public static int getAttInt(XmlNode element,string name){
		if (element.Attributes[name]==null) return 0;
		return int.Parse(element.Attributes[name].Value);
	}
	
	public static float getAttFlt(XmlNode element,string name){
		if (element.Attributes[name]==null) return 0f;
		return float.Parse(element.Attributes[name].Value);
	}

    public static bool getAttBool(XmlNode node,string name){
        return getAttStr(node,name).ToLower()=="true"?true:false;
    }

    public static bool HasAtt(XmlNode element,string name){
        return element.Attributes[name]!=null;
    }
	
	//adding elements
	public static XmlElement addElement(XmlElement element,string name){
		var node=element.OwnerDocument.CreateElement(name);
		element.AppendChild(node);
		return node;
	}
	
	public static XmlElement addElement(XmlElement element,string name,string val){
		var node=element.OwnerDocument.CreateElement(name);
		node.InnerText=val;
		element.AppendChild(node);
		return node;
	}
	
	public static XmlElement addElement(XmlElement element,string name,int val){
		return addElement(element,name,val.ToString());
	}
	
	public static XmlElement addElement(XmlElement element,string name,float val){
		return addElement(element,name,val.ToString());
	}

	//adding attributes	
	public static XmlAttribute addAttribute(XmlElement element,string name,string val){
		var att=element.OwnerDocument.CreateAttribute(name);
		att.Value=val;
		element.Attributes.Append(att);
		return att;
	}

	public static XmlAttribute addAttribute(XmlElement element,string name,int val){
		return addAttribute(element,name,val.ToString());
	}
	

    //adding comments
	public static XmlAttribute addComment(XmlElement element,string name,string val){
		var att=element.OwnerDocument.CreateAttribute(name);
		att.Value=val;
		element.Attributes.Append(att);
		return att;
	}
	
	
	public static XmlComment addComment(XmlElement element,string comment){
		var n=element.OwnerDocument.CreateComment(comment);
		element.AppendChild(n);
		return n;
	}
	
    //finders
	public static List<XmlNode> getChildrenByTag(XmlNode node,string tag){
		List<XmlNode> nodes=new List<XmlNode>();
		foreach (XmlNode n in node){
			if (n.Name==tag){
				nodes.Add(n);
			}
		}
		return nodes;
	}


	public static void readAuto(XmlElement element,object obj){
		foreach (var f in obj.GetType().GetFields()){
			if (f.IsPublic){
				if (element[f.Name]!=null){
					f.SetValue(obj,Convert.ChangeType(element[f.Name].InnerText,f.FieldType));
				}
			}
		}
	}

    public static void readAutoStatic(XmlNode node,Type type){
        XmlAttribute att;
        foreach (var f in type.GetFields()){
            if (f.IsStatic&&f.IsPublic){
                att=node.Attributes[f.Name];
                if (att!=null){
                    f.SetValue(null,Convert.ChangeType(att.Value,f.FieldType));
                }
            }
        }
    }

	public static void writeAuto(XmlElement element,object obj){
		foreach (var f in obj.GetType().GetFields()){
			addElement(element,f.Name,f.GetValue(obj).ToString());
		}
	}

    /// <summary>
    /// Reads an xml doc and assigns its parameters to the public static values of the type
    /// </summary>
    public static void readAutoFileStatic(string path,string file,Type type,string rootNode){

        var Xdoc=GetXmlDocument(path,file);
        var root=Xdoc[rootNode];
        
        readAutoStatic(root,type);

    }

    /// <summary>
    /// Reads an automatically created xml doc and assigns its innards to the object.
    /// </summary>
	public static void readAutoFile(string path,string folder,string file,object obj){
		if (folder!="")
			folder=@"\"+folder;
		if (Directory.Exists(path+folder)){
			file=@"\"+file+".xml";
			
			var Xdoc=new XmlDocument();
			Xdoc.Load(path+folder+file);
			
			var root=Xdoc["Stats"];
			
			readAuto(root,obj);
		}
	}
    /// <summary>
    /// Takes an object and autimagically transform it into an xml doc
    /// Public fields became elements with the value as innerText.
    /// </summary>
	public static void writeAutoFile(string path,string folder,string file,object obj){
		if (folder!="")
			folder=@"\"+folder;
		checkFolder(path+folder);

		file=@"\"+file+".xml";
		var Xdoc=new XmlDocument();
		var root=Xdoc.CreateElement("Stats");
		
		writeAuto(root,obj);
		
		Xdoc.AppendChild(root);
		Xdoc.Save(path+folder+file);
	}
	
	/// <summary>
	/// Creates a folder if it doesn't exist
	/// </summary>
    public static void checkFolder(string path)
    {
		if (!Directory.Exists(path)){
			Directory.CreateDirectory(path);
		}
	}
	
	public static bool checkFile(string path){
		return File.Exists(path);
	}

    static bool LoadFromDisc(){
        bool LoadFromDisc=true;
        #if UNITY_ANDROID||UNITY_WEBPLAYER
        LoadFromDisc=false;
        #endif
        return LoadFromDisc;
    }
    
    /// <summary>
    /// Gets an XML Documents from a path.
    /// From disc if PC. From resources if Android or Webplayer
    /// </summary>
    public static XmlDocument GetXmlDocument(string path,string file){
        var Xdoc=new XmlDocument();
        path=path+"/"+file;
        if (LoadFromDisc()){
            Xdoc.Load(path+".xml");
        }
        else{
            //load from resources
            var asset=Resources.Load(path);

            TextAsset text_asset=(TextAsset)asset;
            Xdoc.Load(text_asset.text.ToStream());
        }
        return Xdoc;
    }

	/// <summary>
	/// Gets all XML Documents from a path recursively.
	/// From disc if PC. From resources if Android or Webplayer
	/// </summary>
	public static List<XmlDocument> GetAllXmlDocuments(string path){
		List<XmlDocument> DOX=new List<XmlDocument>();

		if (LoadFromDisc()){
			XML_Loader.checkFolder(path);
			var files=Directory.GetFiles(path,"*.xml",SearchOption.AllDirectories);
			
			foreach (var f in files)
			{
				var Xdoc=new XmlDocument();
				Xdoc.Load(f);
				DOX.Add(Xdoc);
			}
		}
		else{
			//load from resources
			var text_assets=Resources.LoadAll(path);
			
			foreach (var ta in text_assets)
			{
				TextAsset asset=(TextAsset)ta;
				var Xdoc=new XmlDocument();
				Xdoc.Load(asset.text.ToStream());
				DOX.Add(Xdoc);
			}
		}
		return DOX;
	}
}
