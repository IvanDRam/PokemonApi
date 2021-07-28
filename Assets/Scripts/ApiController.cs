using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class ApiController : MonoBehaviour
{
    #region values
        public RawImage pokeImage;
        public TextMeshProUGUI pokeNameText, pokeNumText;
        public TextMeshProUGUI[] pokeTypeTextA;
        private readonly string basePokeURL ="https://pokeapi.co/api/v2/";
    #endregion

    private void Start()
    {
        //iniciamos valores
        pokeImage.texture=Texture2D.blackTexture;
        pokeNameText.text="";
        pokeNumText.text="";
        foreach(TextMeshProUGUI pokeTypeText in pokeTypeTextA){
            pokeTypeText.text="";
        }
    }

    public void OnButtonRandom(){//apretamos el boton random
        int randomIndex =Random.Range(1,808); //808 pokemones actuales
        pokeImage.texture=Texture2D.blackTexture;
        pokeNameText.text="Loading...";
        pokeNumText.text="#"+randomIndex;

        foreach(TextMeshProUGUI pokeTypeText in pokeTypeTextA){
            pokeTypeText.text="";
        }
        //inicia la Corutina
        StartCoroutine(GetPokemon(randomIndex)); //con el numero random
    }
    public IEnumerator GetPokemon(int PokemonIndex){
        
        //https://pokeapi.co/api/v2/pokemon/62
        string pokemonURL=basePokeURL+"pokemon/"+PokemonIndex.ToString(); //conseguimos la extension URL
        
    //Conseguimos el Web REquest
        UnityWebRequest pokeRequest=UnityWebRequest.Get(pokemonURL);
        yield return pokeRequest.SendWebRequest();

        //revisamos en caso de error
        if(pokeRequest.result==UnityWebRequest.Result.ProtocolError){
            Debug.LogError(pokeRequest.error);
            yield break;
        }

        #region ConseguirValores 
            //pokeInfo tendra todo lo del json
            JSONNode pokeInfo=JSON.Parse(pokeRequest.downloadHandler.text);
            
            //tomamos los valores que nos importen, y los asignamos
            string pokeName=pokeInfo["name"];
            string pokeSpriteURL=pokeInfo["sprites"]["front_default"];

            //poketype puede tener 2 valores, al ser ese caso creamos otro json
            JSONNode pokeTypes=pokeInfo["types"];
            string[] pokeTypeName=new string[pokeTypes.Count];

            for(int i=0,j=pokeTypes.Count-1;i<pokeTypes.Count;i++,j--){
                pokeTypeName[j]=pokeTypes[i]["type"]["name"];
            }
        #endregion
        //Pokemon Sprite
        #region material
            //conseguimos el webRequest
            UnityWebRequest pokeSRequest=UnityWebRequestTexture.GetTexture(pokeSpriteURL);
            yield return pokeSRequest.SendWebRequest();
            
            //revisar en caso de error
            if(pokeSRequest.result!=UnityWebRequest.Result.Success){
                Debug.LogError(pokeRequest.error);
                yield break;
            }

            //creamos la textura
            pokeImage.texture=DownloadHandlerTexture.GetContent(pokeSRequest);
            pokeImage.texture.filterMode=FilterMode.Point;
        #endregion
     
        //pasamos los valores a nuestras variables Text
        pokeNameText.text=CapitalizeFirstLetter(pokeName);
        
        for(int i=0;i<pokeTypeName.Length;i++){
            pokeTypeTextA[i].text=CapitalizeFirstLetter(pokeTypeName[i]);
        }
    }
    
    private string CapitalizeFirstLetter(string str){//cambiamos la primer letra en Mayuscula
        return char.ToUpper(str[0])+str.Substring(1);
    }

}
