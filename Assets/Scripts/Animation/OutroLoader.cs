using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class OutroLoader : MonoBehaviour
{
    [SerializeField] private GameObject outroGroup;
    [SerializeField] private TextMeshProUGUI gameResult;

    public void StartOutro()
    {
        string input = PersistantAnimationData.Instance.AnimationRecord.GameRecord.Metadata.EndState.ToString();
        string output = Regex.Replace(input, "(?<=.)([A-Z])", " $1"); //Adds a space before each uppercase letter
        gameResult.text = output;

        outroGroup.SetActive(true);
        StartCoroutine(SlideOut());
    }

    private IEnumerator SlideOut()
    {
        yield return new WaitForSeconds(2.5f);

        outroGroup.GetComponent<Animator>().SetTrigger("Out");
    }
}