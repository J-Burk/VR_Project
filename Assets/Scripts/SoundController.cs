using UnityEngine;

/*Class handling Sound*/
public class SoundController : MonoBehaviour
{
    //Sound effects
    public AudioSource hammerSuccess;
    public AudioSource hammerFail;
    public AudioSource bellows;
    public AudioSource rhythm;
    public AudioSource grindstone;
    public AudioSource steam;
    public AudioSource failure;
    public AudioSource success;
    public AudioSource fireLoop;
    public AudioSource ambienceMusic;

    //Proximity Sound
    public GameObject player;
    public GameObject oven;
    private float volume = 0.5f;
    const float minDistance = 3.5f;
    const float maxDistance = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onPlaySound += playSoundByName;
    }

    private void Update()
    {
        if(oven != null)
        {
            float distance = Vector3.Distance(player.transform.position, oven.transform.position);
            float volumeAdjustment = Mathf.InverseLerp(minDistance, maxDistance, distance);
            volumeAdjustment = Mathf.Clamp01(volumeAdjustment);
            fireLoop.volume = volumeAdjustment;
        }
    }

    /**
     * plays a sound depending on the given name
     * @param name - name relating to the sound to be played
     */
    void playSoundByName(string name, Vector3 pos)
    {
        switch (name)
        {
            case "HammerSuccess":
                AudioSource.PlayClipAtPoint(hammerSuccess.clip,pos,volume);
                break;
            case "HammerFail":
                AudioSource.PlayClipAtPoint(hammerFail.clip, pos, volume);
                break;
            case "Bellows":
                AudioSource.PlayClipAtPoint(bellows.clip, pos, volume);
                break;
            case "Rhythm":
                AudioSource.PlayClipAtPoint(rhythm.clip, pos, volume);
                break;
            case "Grindstone":
                grindstone.transform.position = pos;
                grindstone.volume = volume;
                grindstone.Play();
                break;
            case "GrindstoneStop":
                grindstone.Stop();
                break;
            case "Steam":
                steam.transform.position = pos;
                steam.volume = volume;
                steam.Play();
                break;
            case "SteamStop":
                steam.Stop();
                break;
            case "Success":
                AudioSource.PlayClipAtPoint(success.clip, pos, volume);
                break;
            case "Failure":
                AudioSource.PlayClipAtPoint(failure.clip, pos, volume);
                break;
            case "HammerOn":
                ambienceMusic.volume = 0.03f;
                break;
            case "HammerOff":
                ambienceMusic.volume = 0.1f;
                break;
        }
    }
}
