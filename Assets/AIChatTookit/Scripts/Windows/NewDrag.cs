namespace AIChatTookit.Scripts.Windows
{
    using UnityEngine;

    using UnityEngine.UI;
    public class NewDrag : MonoBehaviour
    {
        public Transform Target;
            
        public Camera Camera;
            
        bool Drag;

        bool curThrough;

        bool lastThrough;

        private Vector3 lastMousePos;

        public windowTransparent NewTransWin;

        //public Text text;



        void Update()

        {

           //Debug.Log("Input.mousePosition", Input.mousePosition);
            Vector2 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            Target.position = Input.mousePosition;

            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            //Text.text += " |collider:"+collider.gameObject.name;

            
            //处理是否穿透：只要是悬浮在物体上面，就不穿透

            curThrough = collider == null;

            if (curThrough != lastThrough)

            {

                NewTransWin.SetTransparentWindow(curThrough);

                lastThrough = curThrough;

            }



            // 检测鼠标左键

            if (!Input.GetMouseButton(0))

            {

                Drag = false;

            }

            if(Input.GetMouseButtonDown(0))

            { 

                // 检测鼠标是否点击到了当前对象

                if (collider.gameObject ==this.gameObject)

                {

                    Drag = true;

                    lastMousePos = Input.mousePosition;
                   // ShowChatWindow.SetActive(true);
                }
                else
                {
                    //ShowChatWindow.SetActive(false);
                }


            }

     

            // 如果正在拖拽，更新对象的位置

            if (Drag)

            {

                Vector3 offset = Input.mousePosition - lastMousePos;

                NewTransWin.UpdateWindowPosition(offset);

            }

            //text.text = Input.mousePosition.ToString();

        }

    }
    
}