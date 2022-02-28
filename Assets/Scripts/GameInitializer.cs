using UI;
using UI.Views;
using UnityEngine;
using VContainer;

public class GameInitializer : MonoBehaviourDi
{
    [Inject] private ViewRepository viewRepository;
    private void Start()
    {
        viewRepository.Show<MainScreenView, MainScreenView.MainScreenData>(new MainScreenView.MainScreenData());
    }
}