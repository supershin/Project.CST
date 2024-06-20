 const app = {
     init: () => {

     },
     reloadTable: function (table) {
         if (table != undefined) {
             //table.api().clear().draw();
             table.DataTable().clear().draw();
             return;
         }
     }
};
