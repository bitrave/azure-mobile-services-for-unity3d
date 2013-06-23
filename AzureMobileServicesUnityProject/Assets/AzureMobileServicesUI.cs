using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
//using Bitrave.Azure;
//using Bitrave.LiveApps.Azure;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using System.Collections;
//using AzureMobileServices = Bitrave.Azure.AzureMobileServices;

public class AzureMobileServicesUI : MonoBehaviour {

//    // Use this for initialization
//    void Start () {
	
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }

//    public class ToDoItem
//    {
//        public ToDoItem()
//        {
//            Text = "";
//            Complete = false;
//        }

//        public int? Id { get; set; }

//        [JsonProperty(PropertyName = "text")]
//        public string Text { get; set; }

//        [JsonProperty(PropertyName = "complete")]
//        public bool Complete { get; set; }

//        public override string ToString()
//        {
//            return Id + "," + Text + "," + Complete;
//        }
//    }
    public class LevelScore
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public string Level { get; set; }
        public int Bonuses { get; set; }
        public float Time { get; set; }
        public bool JellySaved { get; set; }

        public override string ToString()
        {
            return UserId;
        }
    }

    //private ToDoItem addItem = new ToDoItem(){Text="", Complete = false};
    //private ToDoItem lookupItem = new ToDoItem(){Text="", Complete = false, Id=1};

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(Vector3) && property.PropertyName == "normalized")
            {
                property.ShouldSerialize =
                  instance =>
                  {
                      return false;
                  };
            }

            return property;
        }
    }
//}
//    public void OnGUI()
//    {
//        var screenScale = Screen.width / 820.0f;
//        var scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale, screenScale, screenScale));
//        GUI.matrix = scaledMatrix;

//        GUILayout.BeginVertical();
//        GUILayout.MaxWidth(300);
//        GUILayout.Width(300);

//        AddUI();
//        LookupUI();
//        ReadUI();
//        WhereUI();

//        GUILayout.BeginArea(new Rect(650, 10, 150, 200));
//        if (GUILayout.Button("Launch Auth", GUILayout.Width(150), GUILayout.Height(50)))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.LoginAsync(AuthenticationProvider.Facebook, response =>
//                {
//                    if (response.Status == AzureResponseStatus.Success)
//                    {
//                        Debug.Log(response.ResponseData.UserId + " - " +
//                                  response.ResponseData.MobileServiceAuthenticationToken);
//                    }
//                });
//        }
//        if (GUILayout.Button("Back to Menu", GUILayout.Width(150), GUILayout.Height(50)))
//        {
//            Application.LoadLevel("Menu");
//        }
//        GUILayout.EndArea();

//        GUILayout.EndVertical();
//    }

//    private string _azureEndPoint = "https://bitraveservices.azure-mobile.net/";
//    private string _applicationKey = "ePYhutMVmiUPWhFAJaRYTJsPFTiuAB20";


//    private void AddUI()
//    {
//        GUILayout.BeginArea(new Rect(10, 10, 150, 200));
//        GUILayout.Label("Add Todo Item");
//        GUILayout.Label("Id:" + addItem.Id);
//        addItem.Text = GUILayout.TextField(addItem.Text);
        
//        addItem.Complete = GUILayout.Toggle(addItem.Complete, "Complete");
//        if (GUILayout.Button("Add Item"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            azure.Insert(addItem);
//        }
//        GUILayout.EndArea();
//    }

//    private void LookupUI()
//    {
//        GUILayout.BeginArea(new Rect(170, 10, 150, 250));
//        GUILayout.Label("Lookup/Delete Todo Item");

//        if (lookupItem == null)
//            lookupItem = new ToDoItem() { Text = "", Complete = false, Id = 1 };

//        GUILayout.Label("ID");
//        var idString = lookupItem.Id == null ? "0" : lookupItem.Id.ToString();
//        int itemId = 0;
//        if (int.TryParse(GUILayout.TextField(idString), out itemId))
//        {
//            lookupItem.Id = itemId;
//        }
//        GUILayout.Label("Text");
//        lookupItem.Text = GUILayout.TextField(lookupItem.Text);
//        lookupItem.Complete = GUILayout.Toggle(lookupItem.Complete, "Complete");

//        if (GUILayout.Button("Lookup Item"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            azure.Lookup<ToDoItem>(itemId, response =>
//                {
//                    lookupItem = response.ResponseData;
//                });
//        }
//        if (GUILayout.Button("Delete Item"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            azure.Delete(lookupItem);
//            lookupItem = new ToDoItem() {Text="", Complete = false};
//        }
//        if (GUILayout.Button("Update Item"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            azure.Update(lookupItem);
//        }
//        GUILayout.EndArea();
//    }

//    private List<ToDoItem> allToDoItems = new List<ToDoItem>();
//    private Vector2 scrollPosition = Vector2.zero;
//    private void ReadUI()
//    {
//        GUILayout.BeginArea(new Rect(330, 10, 150, 400));
//        GUILayout.Label("Read Todo Items");

//        if (GUILayout.Button("Lookup Item"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            azure.Read<ToDoItem>(response =>
//                {
//                    allToDoItems = response.ResponseData;
//                });
//        }

//        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
//        foreach (var item in allToDoItems)
//        {
//            GUILayout.Label(item.Id + ":" + item.Text);
//            GUILayout.Toggle(item.Complete, "Complete");
//        }
//        GUILayout.EndScrollView();

//        GUILayout.EndArea();
//    }

//    private List<ToDoItem> whereToDoItems = new List<ToDoItem>();
//    private Vector2 whereScrollPosition = Vector2.zero;
//    private ToDoItem whereItem = new ToDoItem(){Text = "", Complete = false};
//    private void WhereUI()
//    {
//        GUILayout.BeginArea(new Rect(490, 10, 150, 400));
//        GUILayout.Label("Where Todo Items");

//        whereItem.Text = GUILayout.TextField(whereItem.Text);
//        whereItem.Complete = GUILayout.Toggle(whereItem.Complete, "Complete");
        
//        if (GUILayout.Button("Query Items"))
//        {
//            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
//            azure.OnError = response => { Debug.Log(response.Error); };
//            Expression<System.Func<ToDoItem, bool>> myExpression;
//            if (!String.IsNullOrEmpty(whereItem.Text))
//            {
//                myExpression =
//                x => x.Text.Contains(whereItem.Text) && whereItem.Complete == x.Complete;
//            }
//            else
//            {
//                myExpression = x => whereItem.Complete == x.Complete;
//            }
        
//            azure.Where<ToDoItem>(myExpression, response =>
//            {
//                whereToDoItems = response.ResponseData;
//            });
//        }

//        whereScrollPosition = GUILayout.BeginScrollView(whereScrollPosition, false, true);

//        try
//        {
//            foreach (var item in whereToDoItems)
//            {
//                GUILayout.Label(item.Id + ":" + item.Text);
//            }
//        }
//        catch (Exception e)
//        {
//            // Just catches those pesky UI issues
//        }

//        GUILayout.EndScrollView();
       
//        GUILayout.EndArea();
//    }


}
