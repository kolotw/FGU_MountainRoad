// �ޥ� KartGame.KartSystems �R�W�Ŷ�
using KartGame.KartSystems;
using UnityEngine;

public class buttonStartPoint : MonoBehaviour
{
    // �ŧi ArcadeKart �ܶq
    ArcadeKart kart;

    void Start()
    {
        // ��������P�@�� GameObject �W�� ArcadeKart �ե�
        kart = GetComponent<ArcadeKart>();

        // �p�G�䤣�� kart�A���L���~�H��
        if (kart == null)
        {
            Debug.LogError("�䤣�� ArcadeKart �ե�A���ˬd GameObject �t�m");
        }
    }

    // ���_�ӫ��s�ƥ�
    public void ��()
    {
        if (kart != null)
        {
            kart.goWhere("/���_�Ӱ_�I");
        }
    }

    // �j�������s�ƥ�
    public void �j()
    {
        if (kart != null)
        {
            kart.goWhere("/�j�����_�I");
        }
    }
    public void �h()
    {
        if (kart != null)
        {
            kart.goWhere("/�[���x�_�I");
        }
    }
}
