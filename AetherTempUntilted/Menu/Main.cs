using BepInEx;
using HarmonyLib;
using StupidTemplate.Classes;
using StupidTemplate.Notifications;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static AetherTemp.Menu.Buttons;
using static StupidTemplate.Settings;

namespace StupidTemplate.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class Main : MonoBehaviour
    {
        // Constant

        public static float num = 2f;

        public static void MenuDeleteTime()
        {
            if (num == 2f)
                num = 5f; // Long
            else if (num == 5f)
                num = 0.01f; // Fast
            else
                num = 2f; // Default
        }
        public static void Prefix()
        {
            // Initialize Menu
                try
                {
                    bool toOpen = (!rightHanded && ControllerInputPoller.instance.leftControllerSecondaryButton) || (rightHanded && ControllerInputPoller.instance.rightControllerSecondaryButton);
                    bool keyboardOpen = UnityInput.Current.GetKey(keyboardButton);

                    if (menu == null)
                    {
                        if (toOpen || keyboardOpen)
                        {
                            CreateMenu();
                            RecenterMenu(rightHanded, keyboardOpen);
                            if (reference == null)
                            {
                                CreateReference(rightHanded);
                            }
                        }
                    }
                    else
                    {
                        if ((toOpen || keyboardOpen))
                        {
                            RecenterMenu(rightHanded, keyboardOpen);
                        }
                        else
                        {
                            GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);

                            Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                            if (rightHanded)
                            {
                                comp.velocity = GorillaLocomotion.GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                            }
                            else
                            {
                                comp.velocity = GorillaLocomotion.GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                            }

                            UnityEngine.Object.Destroy(menu, 2);
                            menu = null;

                            UnityEngine.Object.Destroy(reference);
                            reference = null;
                        }
                    }
                }
                catch (Exception exc)
                {
                    UnityEngine.Debug.LogError(string.Format("{0} // Error initializing at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }

            // Constant
                try
                {
                    // Pre-Execution
                        if (fpsObject != null)
                        {
                            fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                        }

                    // Execute Enabled mods
                        foreach (ButtonInfo[] buttonlist in buttons)
                        {
                            foreach (ButtonInfo v in buttonlist)
                            {
                                if (v.enabled)
                                {
                                    if (v.method != null)
                                    {
                                        try
                                        {
                                            v.method.Invoke();
                                        }
                                        catch (Exception exc)
                                        {
                                            UnityEngine.Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, v.buttonText, exc.StackTrace, exc.Message));
                                        }
                                    }
                                }
                            }
                        }
                } catch (Exception exc)
                {
                    UnityEngine.Debug.LogError(string.Format("{0} // Error with executing mods at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }
        }

        // Functions
        public static void CreateMenu()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);

            GameObject menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menuBackground.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menuBackground.GetComponent<BoxCollider>());
            menuBackground.transform.parent = menu.transform;
            menuBackground.transform.rotation = Quaternion.identity;
            menuBackground.transform.localScale = menuSize;
            menuBackground.transform.position = new Vector3(0.05f, 0f, 0f);

            menuBackground.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;

            menuBackground.GetComponent<Renderer>().enabled = false;

            float bevel = 0.02f;

            Renderer ToRoundRenderer = menuBackground.GetComponent<Renderer>();


            GameObject BaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseA.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(BaseA.GetComponent<Collider>());
            BaseA.transform.parent = menu.transform;
            BaseA.transform.rotation = Quaternion.identity;
            BaseA.transform.localPosition = menuBackground.transform.localPosition;
            BaseA.transform.localScale = menuBackground.transform.localScale + new Vector3(0f, bevel * -2.55f, 0f);

            GameObject BaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseB.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(BaseB.GetComponent<Collider>());
            BaseB.transform.parent = menu.transform;
            BaseB.transform.rotation = Quaternion.identity;
            BaseB.transform.localPosition = menuBackground.transform.localPosition;
            BaseB.transform.localScale = menuBackground.transform.localScale + new Vector3(0f, 0f, -bevel * 2f);

            GameObject RoundCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerA.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(RoundCornerA.GetComponent<Collider>());
            RoundCornerA.transform.parent = menu.transform;
            RoundCornerA.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            RoundCornerA.transform.localPosition = menuBackground.transform.localPosition + new Vector3(0f, (menuBackground.transform.localScale.y / 2f) - (bevel * 1.275f), (menuBackground.transform.localScale.z / 2f) - bevel);
            RoundCornerA.transform.localScale = new Vector3(bevel * 2.55f, menuBackground.transform.localScale.x / 2f, bevel * 2f);

            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(RoundCornerB.GetComponent<Collider>());
            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            RoundCornerB.transform.localPosition = menuBackground.transform.localPosition + new Vector3(0f, -(menuBackground.transform.localScale.y / 2f) + (bevel * 1.275f), (menuBackground.transform.localScale.z / 2f) - bevel);
            RoundCornerB.transform.localScale = new Vector3(bevel * 2.55f, menuBackground.transform.localScale.x / 2f, bevel * 2f);

            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(RoundCornerC.GetComponent<Collider>());
            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            RoundCornerC.transform.localPosition = menuBackground.transform.localPosition + new Vector3(0f, (menuBackground.transform.localScale.y / 2f) - (bevel * 1.275f), -(menuBackground.transform.localScale.z / 2f) + bevel);
            RoundCornerC.transform.localScale = new Vector3(bevel * 2.55f, menuBackground.transform.localScale.x / 2f, bevel * 2f);

            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(RoundCornerD.GetComponent<Collider>());
            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            RoundCornerD.transform.localPosition = menuBackground.transform.localPosition + new Vector3(0f, -(menuBackground.transform.localScale.y / 2f) + (bevel * 1.275f), -(menuBackground.transform.localScale.z / 2f) + bevel);
            RoundCornerD.transform.localScale = new Vector3(bevel * 2.55f, menuBackground.transform.localScale.x / 2f, bevel * 2f);

            GameObject[] ToChange = new GameObject[]
            {
    BaseA,
    BaseB,
    RoundCornerA,
    RoundCornerB,
    RoundCornerC,
    RoundCornerD
            };

            foreach (GameObject obj in ToChange)
            {
                obj.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
            }

            ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
                colorChanger.colorInfo = backgroundColor;
                colorChanger.Start();

            // Canvas
                canvasObject = new GameObject();
                canvasObject.transform.parent = menu.transform;
                Canvas canvas = canvasObject.AddComponent<Canvas>();
                CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasScaler.dynamicPixelsPerUnit = 2000f;

            // Title and FPS
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = PluginInfo.Name;
                text.fontSize = 1;
                text.color = textColors[0];
                text.supportRichText = true;
                text.fontStyle = FontStyle.Italic;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.18f, 0.04f);
                component.position = new Vector3(0.06f, 0f, 0.17f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (fpsCounter)
                {
                    fpsObject = new GameObject
                    {
                        transform =
                    {
                        parent = canvasObject.transform
                    }
                    }.AddComponent<Text>();
                    fpsObject.font = currentFont;
                    fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                    fpsObject.color = textColors[0];
                    fpsObject.fontSize = 1;
                    fpsObject.supportRichText = true;
                    fpsObject.fontStyle = FontStyle.Italic;
                    fpsObject.alignment = TextAnchor.MiddleCenter;
                    fpsObject.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
                    fpsObject.resizeTextForBestFit = true;
                    fpsObject.resizeTextMinSize = 0;
                    RectTransform component2 = fpsObject.GetComponent<RectTransform>();
                    component2.localPosition = Vector3.zero;
                    component2.sizeDelta = new Vector2(0.09f, 0.01f);
                    component2.position = new Vector3(0.06f, 0.12f, -0.185f);
                    component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }

                    if (disconnectButton)
                    {

                GameObject disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(KeyCode.Q))
                {
                    disconnectbutton.layer = 2;
                }
                UnityEngine.Object.Destroy(disconnectbutton.GetComponent<Rigidbody>());
                disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectbutton.transform.parent = menu.transform;
                disconnectbutton.transform.rotation = Quaternion.identity;
                disconnectbutton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
                Color buttonColor = buttonColors[0].colors[0].color;
                disconnectbutton.GetComponent<Renderer>().material.color = buttonColor;
                disconnectbutton.AddComponent<Classes.Button>().relatedText = "Disconnect";
                disconnectbutton.GetComponent<Renderer>().enabled = false; 

                GameObject ButtonBaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(ButtonBaseA.GetComponent<Collider>());
                ButtonBaseA.GetComponent<Renderer>().material.color = buttonColor;
                ButtonBaseA.transform.parent = menu.transform;
                ButtonBaseA.transform.rotation = Quaternion.identity;
                ButtonBaseA.transform.localPosition = disconnectbutton.transform.localPosition;
                ButtonBaseA.transform.localScale = disconnectbutton.transform.localScale + new Vector3(0f, -bevel * 2.55f, 0f);

                GameObject ButtonBaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(ButtonBaseB.GetComponent<Collider>());
                ButtonBaseB.GetComponent<Renderer>().material.color = buttonColor;
                ButtonBaseB.transform.parent = menu.transform;
                ButtonBaseB.transform.rotation = Quaternion.identity;
                ButtonBaseB.transform.localPosition = disconnectbutton.transform.localPosition;
                ButtonBaseB.transform.localScale = disconnectbutton.transform.localScale + new Vector3(0f, 0f, -bevel * 2f);

                GameObject ButtonCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(ButtonCornerA.GetComponent<Collider>());
                ButtonCornerA.GetComponent<Renderer>().material.color = buttonColor;
                ButtonCornerA.transform.parent = menu.transform;
                ButtonCornerA.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                ButtonCornerA.transform.localPosition = disconnectbutton.transform.localPosition + new Vector3(0f, (disconnectbutton.transform.localScale.y / 2f) - (bevel * 1.275f), (disconnectbutton.transform.localScale.z / 2f) - bevel);
                ButtonCornerA.transform.localScale = new Vector3(bevel * 2.55f, disconnectbutton.transform.localScale.x / 2f, bevel * 2f);


                GameObject ButtonCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(ButtonCornerB.GetComponent<Collider>());
                ButtonCornerB.GetComponent<Renderer>().material.color = buttonColor;
                ButtonCornerB.transform.parent = menu.transform;
                ButtonCornerB.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                ButtonCornerB.transform.localPosition = disconnectbutton.transform.localPosition + new Vector3(0f, -(disconnectbutton.transform.localScale.y / 2f) + (bevel * 1.275f), (disconnectbutton.transform.localScale.z / 2f) - bevel);
                ButtonCornerB.transform.localScale = new Vector3(bevel * 2.55f, disconnectbutton.transform.localScale.x / 2f, bevel * 2f);

                GameObject ButtonCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(ButtonCornerC.GetComponent<Collider>());
                ButtonCornerC.GetComponent<Renderer>().material.color = buttonColor;
                ButtonCornerC.transform.parent = menu.transform;
                ButtonCornerC.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                ButtonCornerC.transform.localPosition = disconnectbutton.transform.localPosition + new Vector3(0f, (disconnectbutton.transform.localScale.y / 2f) - (bevel * 1.275f), -(disconnectbutton.transform.localScale.z / 2f) + bevel);
                ButtonCornerC.transform.localScale = new Vector3(bevel * 2.55f, disconnectbutton.transform.localScale.x / 2f, bevel * 2f);

                GameObject ButtonCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(ButtonCornerD.GetComponent<Collider>());
                ButtonCornerD.GetComponent<Renderer>().material.color = buttonColor;
                ButtonCornerD.transform.parent = menu.transform;
                ButtonCornerD.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                ButtonCornerD.transform.localPosition = disconnectbutton.transform.localPosition + new Vector3(0f, -(disconnectbutton.transform.localScale.y / 2f) + (bevel * 1.275f), -(disconnectbutton.transform.localScale.z / 2f) + bevel);
                ButtonCornerD.transform.localScale = new Vector3(bevel * 2.55f, disconnectbutton.transform.localScale.x / 2f, bevel * 2f);


                GameObject[] disconnectButtonParts = new GameObject[]
                {
    ButtonBaseA,
    ButtonBaseB,
    ButtonCornerA,
    ButtonCornerB,
    ButtonCornerC,
    ButtonCornerD
                };


                colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                        colorChanger.colorInfo = buttonColors[0];
                        colorChanger.Start();

                        Text discontext = new GameObject
                        {
                            transform =
                            {
                                parent = canvasObject.transform
                            }
                        }.AddComponent<Text>();
                        discontext.text = "Disconnect";
                        discontext.font = currentFont;
                        discontext.fontSize = 1;
                        discontext.color = textColors[0];
                        discontext.alignment = TextAnchor.MiddleCenter;
                        discontext.resizeTextForBestFit = true;
                        discontext.resizeTextMinSize = 0;

                        RectTransform rectt = discontext.GetComponent<RectTransform>();
                        rectt.localPosition = Vector3.zero;
                        rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                        rectt.localPosition = new Vector3(0.064f, 0f, 0.23f);
                        rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                    }

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
            {
                gameObject.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0.65f, 0);

            gameObject.GetComponent<Renderer>().enabled = false;

            gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";

            GameObject fillY = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(fillY.GetComponent<Collider>());
            fillY.GetComponent<Renderer>().material.color = ButtonColor;
            fillY.transform.parent = menu.transform;
            fillY.transform.rotation = Quaternion.identity;
            fillY.transform.localPosition = gameObject.transform.localPosition;
            fillY.transform.localScale = gameObject.transform.localScale + new Vector3(0f, -bevel * 2.55f, 0f);

            GameObject fillZ = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(fillZ.GetComponent<Collider>());
            fillZ.GetComponent<Renderer>().material.color = ButtonColor;
            fillZ.transform.parent = menu.transform;
            fillZ.transform.rotation = Quaternion.identity;
            fillZ.transform.localPosition = gameObject.transform.localPosition;
            fillZ.transform.localScale = gameObject.transform.localScale + new Vector3(0f, 0f, -bevel * 2f);


            Vector3 center = gameObject.transform.localPosition;
            Vector3 scale = gameObject.transform.localScale;

            void CreateCorner(Vector3 offset, string name)
            {
                GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(corner.GetComponent<Collider>());
                corner.name = name;
                corner.GetComponent<Renderer>().material.color = ButtonColor;
                corner.transform.parent = menu.transform;
                corner.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                corner.transform.localPosition = center + offset;
                corner.transform.localScale = new Vector3(bevel * 2.55f, scale.x / 2f, bevel * 2f);
            }

            CreateCorner(new Vector3(0f, (scale.y / 2f) - (bevel * 1.275f), (scale.z / 2f) - bevel), "CornerTopFront");
            CreateCorner(new Vector3(0f, -(scale.y / 2f) + (bevel * 1.275f), (scale.z / 2f) - bevel), "CornerBottomFront");
            CreateCorner(new Vector3(0f, (scale.y / 2f) - (bevel * 1.275f), -(scale.z / 2f) + bevel), "CornerTopBack");
            CreateCorner(new Vector3(0f, -(scale.y / 2f) + (bevel * 1.275f), -(scale.z / 2f) + bevel), "CornerBottomBack");



            colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colorInfo = buttonColors[0];
                    colorChanger.Start();

                    text = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    text.font = currentFont;
                    text.text = "<";
                    text.fontSize = 1;
                    text.color = textColors[0];
                    text.alignment = TextAnchor.MiddleCenter;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 0;
                    component = text.GetComponent<RectTransform>();
                    component.localPosition = Vector3.zero;
                    component.sizeDelta = new Vector2(0.2f, 0.03f);
                    component.localPosition = new Vector3(0.064f, 0.195f, 0f);
                    component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            GameObject nextPageButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
            {
                nextPageButton.layer = 2;
            }
            UnityEngine.Object.Destroy(nextPageButton.GetComponent<Rigidbody>());
            nextPageButton.GetComponent<BoxCollider>().isTrigger = true;
            nextPageButton.transform.parent = menu.transform;
            nextPageButton.transform.rotation = Quaternion.identity;
            nextPageButton.transform.localScale = new Vector3(0.09f, 0.2f, 0.9f);
            nextPageButton.transform.localPosition = new Vector3(0.56f, -0.65f, 0);
            nextPageButton.GetComponent<Renderer>().enabled = false;
            nextPageButton.AddComponent<Classes.Button>().relatedText = "NextPage";


            Color buttonFillColor = buttonColors[0].colors[0].color;
            float cornerRadius = 0.01f;

            GameObject nextPageFillY = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(nextPageFillY.GetComponent<Collider>());
            nextPageFillY.GetComponent<Renderer>().material.color = buttonFillColor;
            nextPageFillY.transform.parent = menu.transform;
            nextPageFillY.transform.rotation = Quaternion.identity;
            nextPageFillY.transform.localPosition = nextPageButton.transform.localPosition;
            nextPageFillY.transform.localScale = nextPageButton.transform.localScale + new Vector3(0f, -cornerRadius * 2.55f, 0f);

            GameObject nextPageFillZ = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(nextPageFillZ.GetComponent<Collider>());
            nextPageFillZ.GetComponent<Renderer>().material.color = buttonFillColor;
            nextPageFillZ.transform.parent = menu.transform;
            nextPageFillZ.transform.rotation = Quaternion.identity;
            nextPageFillZ.transform.localPosition = nextPageButton.transform.localPosition;
            nextPageFillZ.transform.localScale = nextPageButton.transform.localScale + new Vector3(0f, 0f, -cornerRadius * 2f);

            Vector3 buttonCenter = nextPageButton.transform.localPosition;
            Vector3 buttonScale = nextPageButton.transform.localScale;

            void CreateButtonCorner(Vector3 offset, string cornerName)
            {
                GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(corner.GetComponent<Collider>());
                corner.name = cornerName;
                corner.GetComponent<Renderer>().material.color = buttonFillColor;
                corner.transform.parent = menu.transform;
                corner.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                corner.transform.localPosition = buttonCenter + offset;
                corner.transform.localScale = new Vector3(cornerRadius * 2.55f, buttonScale.x / 2f, cornerRadius * 2f);
            }

            CreateButtonCorner(new Vector3(0f, (buttonScale.y / 2f) - (cornerRadius * 1.275f), (buttonScale.z / 2f) - cornerRadius), "TopFrontCorner");
            CreateButtonCorner(new Vector3(0f, -(buttonScale.y / 2f) + (cornerRadius * 1.275f), (buttonScale.z / 2f) - cornerRadius), "BottomFrontCorner");
            CreateButtonCorner(new Vector3(0f, (buttonScale.y / 2f) - (cornerRadius * 1.275f), -(buttonScale.z / 2f) + cornerRadius), "TopBackCorner");
            CreateButtonCorner(new Vector3(0f, -(buttonScale.y / 2f) + (cornerRadius * 1.275f), -(buttonScale.z / 2f) + cornerRadius), "BottomBackCorner");


            colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colorInfo = buttonColors[0];
                    colorChanger.Start();

                    text = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    text.font = currentFont;
                    text.text = ">";
                    text.fontSize = 1;
                    text.color = textColors[0];
                    text.alignment = TextAnchor.MiddleCenter;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 0;
                    component = text.GetComponent<RectTransform>();
                    component.localPosition = Vector3.zero;
                    component.sizeDelta = new Vector2(0.2f, 0.03f);
                    component.localPosition = new Vector3(0.064f, -0.195f, 0f);
                    component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));


                    ButtonInfo[] activeButtons = buttons[buttonsType].Skip(pageNumber * buttonsPerPage).Take(buttonsPerPage).ToArray();
                    for (int i = 0; i < activeButtons.Length; i++)
                    {
                        CreateButton(i * 0.1f, activeButtons[i]);
                    }
        }

        public static void CreateButton(float offset, ButtonInfo method)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(KeyCode.Q))
            {
                gameObject.layer = 2;
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;

            gameObject.transform.localScale = new Vector3(0.06f, 0.9f, 0.09f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.32f - offset);

            gameObject.GetComponent<Renderer>().enabled = false;

            float Bevel = 0.02f;
            GameObject BaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseA.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(BaseA.GetComponent<Collider>());
            BaseA.transform.parent = menu.transform;
            BaseA.transform.rotation = Quaternion.identity;
            BaseA.transform.localPosition = gameObject.transform.localPosition;
            BaseA.transform.localScale = gameObject.transform.localScale + new Vector3(0f, Bevel * -2.55f, 0f);
            BaseA.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject BaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseB.GetComponent<Renderer>().enabled = true;
            UnityEngine.Object.Destroy(BaseB.GetComponent<Collider>());
            BaseB.transform.parent = menu.transform;
            BaseB.transform.rotation = Quaternion.identity;
            BaseB.transform.localPosition = gameObject.transform.localPosition;
            BaseB.transform.localScale = gameObject.transform.localScale + new Vector3(0f, 0f, -Bevel * 2f);
            BaseB.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject RoundCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerA.GetComponent<Renderer>().enabled = true;  
            UnityEngine.Object.Destroy(RoundCornerA.GetComponent<Collider>());
            RoundCornerA.transform.parent = menu.transform;
            RoundCornerA.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerA.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, (gameObject.transform.localScale.y / 2f) - (Bevel * 1.275f), (gameObject.transform.localScale.z / 2f) - Bevel);
            RoundCornerA.transform.localScale = new Vector3(Bevel * 2.55f, gameObject.transform.localScale.x / 2f, Bevel * 2f);
            RoundCornerA.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = true;  
            UnityEngine.Object.Destroy(RoundCornerB.GetComponent<Collider>());
            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerB.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, -(gameObject.transform.localScale.y / 2f) + (Bevel * 1.275f), (gameObject.transform.localScale.z / 2f) - Bevel);
            RoundCornerB.transform.localScale = new Vector3(Bevel * 2.55f, gameObject.transform.localScale.x / 2f, Bevel * 2f);
            RoundCornerB.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = true; 
            UnityEngine.Object.Destroy(RoundCornerC.GetComponent<Collider>());
            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerC.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, (gameObject.transform.localScale.y / 2f) - (Bevel * 1.275f), -(gameObject.transform.localScale.z / 2f) + Bevel);
            RoundCornerC.transform.localScale = new Vector3(Bevel * 2.55f, gameObject.transform.localScale.x / 2f, Bevel * 2f);
            RoundCornerC.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = true; 
            UnityEngine.Object.Destroy(RoundCornerD.GetComponent<Collider>());
            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerD.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, -(gameObject.transform.localScale.y / 2f) + (Bevel * 1.275f), -(gameObject.transform.localScale.z / 2f) + Bevel);
            RoundCornerD.transform.localScale = new Vector3(Bevel * 2.55f, gameObject.transform.localScale.x / 2f, Bevel * 2f);
            RoundCornerD.GetComponent<Renderer>().material.color = ButtonColor;

            gameObject.AddComponent<Classes.Button>().relatedText = method.buttonText;
            gameObject.GetComponent<Renderer>().material.color = ButtonColor;

            GameObject[] ToChange = new GameObject[]
            {
    BaseA,
    BaseB,
    RoundCornerA,
    RoundCornerB,
    RoundCornerC,
    RoundCornerD
            };

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            if (method.enabled)
            {
                colorChanger.colorInfo = buttonColors[1];
            }
            else
            {
                colorChanger.colorInfo = buttonColors[0];
            }
            colorChanger.Start();

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.buttonText;
            if (method.overlapText != null)
            {
                text.text = method.overlapText;
            }
            text.supportRichText = true;
            text.fontSize = 1;
            if (method.enabled)
            {
                text.color = textColors[1];
            }
            else
            {
                text.color = textColors[0];
            }
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            component.localPosition = new Vector3(.064f, 0, .125f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void RecreateMenu()
        {
            if (menu != null)
            {
                UnityEngine.Object.Destroy(menu);
                menu = null;

                CreateMenu();
                RecenterMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
            }
        }

        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (!isKeyboardCondition)
            {
                if (!isRightHanded)
                {
                    menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                }
                else
                {
                    menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    Vector3 rotation = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                    rotation += new Vector3(0f, 0f, 180f);
                    menu.transform.rotation = Quaternion.Euler(rotation);
                }
            }
            else
            {
                try
                {
                    TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                }
                catch { }

                GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

                if (TPC != null)
                {
                    TPC.transform.position = new Vector3(-999f, -999f, -999f);
                    TPC.transform.rotation = Quaternion.identity;
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = (TPC.transform.position + (Vector3.Scale(TPC.transform.forward, new Vector3(0.5f, 0.5f, 0.5f)))) + (Vector3.Scale(TPC.transform.up, new Vector3(-0.02f, -0.02f, -0.02f)));
                    Vector3 rot = TPC.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x - 90, rot.y + 90, rot.z);
                    menu.transform.rotation = Quaternion.Euler(rot);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            RaycastHit hit;
                            bool worked = Physics.Raycast(ray, out hit, 100);
                            if (worked)
                            {
                                Classes.Button collide = hit.transform.gameObject.GetComponent<Classes.Button>();
                                if (collide != null)
                                {
                                    collide.OnTriggerEnter(buttonCollider);
                                }
                            }
                        }
                        else
                        {
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                        }
                    }
                }
            }
        }

        public static void CreateReference(bool isRightHanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (isRightHanded)
            {
                reference.transform.parent = GorillaTagger.Instance.leftHandTransform;
            }
            else
            {
                reference.transform.parent = GorillaTagger.Instance.rightHandTransform;
            }
            reference.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();

            ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
            colorChanger.colorInfo = backgroundColor;
            colorChanger.Start();
        }

        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[buttonsType].Length + buttonsPerPage - 1) / buttonsPerPage) - 1;
            if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                {
                    pageNumber = lastPage;
                }
            } else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                    {
                        pageNumber = 0;
                    }
                } else
                {
                    ButtonInfo target = GetIndex(buttonText);
                    if (target != null)
                    {
                        if (target.isTogglable)
                        {
                            target.enabled = !target.enabled;
                            if (target.enabled)
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                if (target.enableMethod != null)
                                {
                                    try { target.enableMethod.Invoke(); } catch { }
                                }
                            }
                            else
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                if (target.disableMethod != null)
                                {
                                    try { target.disableMethod.Invoke(); } catch { }
                                }
                            }
                        }
                        else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                            if (target.method != null)
                            {
                                try { target.method.Invoke(); } catch { }
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(buttonText + " does not exist");
                    }
                }
            }
            RecreateMenu();
        }

        public static GradientColorKey[] GetSolidGradient(Color color)
        {
            return new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) };
        }

        public static ButtonInfo GetIndex(string buttonText)
        {
            foreach (ButtonInfo[] buttons in buttons)
            {
                foreach (ButtonInfo button in buttons)
                {
                    if (button.buttonText == buttonText)
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        // Variables
            // Important
                // Objects
                    public static GameObject menu;
                    public static GameObject menuBackground;   
                    public static GameObject reference;
                    public static GameObject canvasObject;

                    public static SphereCollider buttonCollider;
                    public static Camera TPC;
                    public static Text fpsObject;

        // Data
            public static int pageNumber = 0;
            public static int buttonsType = 0;
    }
}
