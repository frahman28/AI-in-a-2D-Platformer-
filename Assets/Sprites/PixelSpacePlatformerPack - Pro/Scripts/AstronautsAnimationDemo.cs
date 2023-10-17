using UnityEngine;

public class AstronautsAnimationDemo : MonoBehaviour
{
    public GameObject Astro_1, Astro_2, Astro_3, Astro_4, Astro_5, Astro_6;

    public void AstroStay()
    {
        Astro_1.GetComponent<Animator>().SetInteger("AstroAnim_1", 0);
        Astro_2.GetComponent<Animator>().SetInteger("AstroAnim_2", 0);
        Astro_3.GetComponent<Animator>().SetInteger("AstroAnim_3", 0);
        Astro_4.GetComponent<Animator>().SetInteger("AstroAnim_4", 0);
        Astro_5.GetComponent<Animator>().SetInteger("AstroAnim_5", 0);
        Astro_6.GetComponent<Animator>().SetInteger("AstroAnim_6", 0);
    }
    public void AstroWalk()
    {
        Astro_1.GetComponent<Animator>().SetInteger("AstroAnim_1", 1);
        Astro_2.GetComponent<Animator>().SetInteger("AstroAnim_2", 1);
        Astro_3.GetComponent<Animator>().SetInteger("AstroAnim_3", 1);
        Astro_4.GetComponent<Animator>().SetInteger("AstroAnim_4", 1);
        Astro_5.GetComponent<Animator>().SetInteger("AstroAnim_5", 1);
        Astro_6.GetComponent<Animator>().SetInteger("AstroAnim_6", 1);
    }
    public void AstroJump()
    {
        Astro_1.GetComponent<Animator>().SetInteger("AstroAnim_1", 2);
        Astro_2.GetComponent<Animator>().SetInteger("AstroAnim_2", 2);
        Astro_3.GetComponent<Animator>().SetInteger("AstroAnim_3", 2);
        Astro_4.GetComponent<Animator>().SetInteger("AstroAnim_4", 2);
        Astro_5.GetComponent<Animator>().SetInteger("AstroAnim_5", 2);
        Astro_6.GetComponent<Animator>().SetInteger("AstroAnim_6", 2);
    }
    public void AstroLadder()
    {
        Astro_1.GetComponent<Animator>().SetInteger("AstroAnim_1", 3);
        Astro_2.GetComponent<Animator>().SetInteger("AstroAnim_2", 3);
        Astro_3.GetComponent<Animator>().SetInteger("AstroAnim_3", 3);
        Astro_4.GetComponent<Animator>().SetInteger("AstroAnim_4", 3);
        Astro_5.GetComponent<Animator>().SetInteger("AstroAnim_5", 3);
        Astro_6.GetComponent<Animator>().SetInteger("AstroAnim_6", 3);
    }
}
