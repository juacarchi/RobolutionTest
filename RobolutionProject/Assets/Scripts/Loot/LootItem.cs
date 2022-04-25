using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    public LayerMask playerLayerMask;
    public float radius;
    public float speed;
    bool isInRange;
    bool checkPlayer;
    bool isRendering;
    float curtTime = 0f;
    bool instantiated;
    // Update is called once per frame

    public void DesactivateGO()
    {
        this.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!checkPlayer)
        {
            isInRange = Physics.CheckSphere(this.transform.position, radius, playerLayerMask);
        }

        if (isInRange)
        {
            checkPlayer = true;
            Vector3 direction = PlayerController.instance.transform.position - this.transform.position;
            transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
        }

        //CAMERA
        Vector2 vec2 = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);

        if (PlayerController.instance.health < PlayerController.instance.healthMax && this.CompareTag("MedicalKit"))
        {
            if (IsInView(transform.position))
            {
                UIManager.instance.DesactivateSpriteLoot();
                instantiated = false;
            }
            else
            {
                if (!instantiated)
                {
                    UIManager.instance.ActiveSpriteLoot(this.gameObject);
                    instantiated = true;
                }
            }
        }

        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.CompareTag("MedicalKit"))
            {
                PlayerController.instance.IncreaseHP();
            }
            else if (this.CompareTag("Money"))
            {
                GameManager.instance.SetGold(GameManager.instance.GetGold() + 1);
            }
            else if (this.CompareTag("Chip"))
            {
                GameManager.instance.SetChips(GameManager.instance.GetChips() + 1);
            }
            DesactivateGO();
        }
    }

    void OnWillRenderObject()
    {
        curtTime = Time.time;
    }
    public bool IsInView(Vector3 worldPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir); // Determinar si el objeto está frente a la cámara  

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }

}
