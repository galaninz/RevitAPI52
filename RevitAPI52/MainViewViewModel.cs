using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI52
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }
        public List<Autodesk.Revit.DB.Element> PickedObjects { get; } = new List<Autodesk.Revit.DB.Element>();
        public List<WallType> WallTypes { get; } = new List<WallType>();
        public WallType SelectedWallType { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            //SelectCommand = new DelegateCommand(OnSelectCommand);
            SaveCommand = new DelegateCommand(OnSaveComman);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallTypes = WallUtils.GetWallType(commandData);
        }

        private void OnSaveComman()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallType == null)
                return;

            using (var ts = new Autodesk.Revit.DB.Transaction(doc, "Set wall type"))
            {
                ts.Start();
                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;
                        oWall.ChangeTypeId(SelectedWallType.Id);
                    }
                }
                ts.Commit();
            }

        }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        //public event EventHandler HideRequest;
        //private void RaiseHideRequest()
        //{
        //    HideRequest?.Invoke(this, EventArgs.Empty);
        //}

        //public event EventHandler ShowRequest;
        //private void RaiseShowRequest()
        //{
        //    ShowRequest?.Invoke(this, EventArgs.Empty);
        //}

        //private void OnSelectCommand()
        //{
        //    RaiseHideRequest();

        //    Autodesk.Revit.DB.Element oElement = SelectionUtils.PickObject(_commandData);

        //    TaskDialog.Show("Сообщение", $"ID: {oElement.Id}");

        //    RaiseShowRequest();
        //}

    }
}
