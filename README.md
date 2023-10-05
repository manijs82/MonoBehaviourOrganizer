# MonoBehaviourOrganizer

A custom organized [hierarchy](#the-hierarchy) and [inspector](#the-inspector) tool for unity where you only see what you want from the default view plus a simple [placement tool](#placement-tool). <br>
<img src="https://github.com/manijs82/MonoBehaviourOrganizer/assets/57400375/fb03e85f-ee80-49b3-a50e-3edce71472d0" alt="step1" width="500"/>

## The Hierarchy

The Hierarchy only shows the gameobjects with the types that you have added to the valid types. As a result you will see a striped down view of the defualt unity hierarchy.
| ![hierarchy comparision](https://github.com/manijs82/MonoBehaviourOrganizer/assets/57400375/08b90891-92e1-422e-b7e0-662d29c8f9aa) | 
|:--:| 
On my window only the root objects that contain a object that has the PlaceableObject component are shown and when you select the root object you see the list of PlaceableObjects on the right <br>

## The Inspector

The Inspector for each gameobject is shown when you open the foldout of object. The contents are defined in code by a custom method and property attribute.
| ![inspector](https://github.com/manijs82/MonoBehaviourOrganizer/assets/57400375/0d6a9a6c-e405-43e4-a5e3-dfe723542706) | 
|:--:| 
Here the property FloatVar and the two methods SetFloat and PrintFloat are marked with my custom attribute and are shown in the inspector <br>

# Placement Tool
The Placement Tool is a simple click to place which places a selected prefab and orient it to the normal of the surface if you want.


![PlacementTool](https://github.com/manijs82/MonoBehaviourOrganizer/assets/57400375/4e3ad1cc-5ba4-4f7e-b0a0-14f5f8a59688)
