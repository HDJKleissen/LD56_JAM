using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStrike : MonoBehaviour
{
    public BossController boss;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite[] sprites = new Sprite[2];
    [SerializeField] Collider damageCollider;

    [SerializeField] float frame1Time;
    [SerializeField] float frame2Time;
    [SerializeField] float damage;
    // Start is called before the first frame update
    void Start()
    {
        damageCollider.enabled = false;
        StartCoroutine(Strike());
    }

    IEnumerator Strike()
    {
        sprite.sprite = sprites[0];
        yield return new WaitForSeconds(frame1Time);
        sprite.sprite = sprites[1];
        damageCollider.enabled = true;
        yield return new WaitForSeconds(frame2Time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        NPCCharacter character = other.GetComponentInChildren<NPCCharacter>();
        if (character && character.GetComponent<BossController>() == null)
        {
            character.Damage(damage, boss, CharacterTeam.ENEMY);
        }
    }
}
