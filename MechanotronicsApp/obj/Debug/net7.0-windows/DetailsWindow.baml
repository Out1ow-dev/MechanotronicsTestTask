   M S B A M L   `   `   `  ����  MechanotronicsApp%    MechanotronicsApp.DetailsWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Детали��5   	   $	��450��6   $	��800��6%   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��10q�6;   .+�5      �� -   5         ��Name$ RefreshButton��5      $��Обновить��6,   $��5q�6?   6J   �� 5      -   5        $ ClearButton��5      $��Очистить��6*   $��5q�6=   6H   �� 5      -   5        $ OpenTableButton��5      $6��0Открыть детальную таблицу��6.   $��5q�6R   6]   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ DetailsDataGrid��5      $��1��5      $��10q�5         AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5         CanUserAddRows$ False��5         CanUserDeleteRows$ False��5      	   CanUserReorderColumns$	 False��5      
   CanUserSortColumns$

 True��5         SelectionMode$ Single=�5         SelectionUnit$ FullRow=�5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Дата��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 5   #      
IsReadOnly$
 True��5    #   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5   #   �� *Date+ �� StringFormat$ {0:dd.MM.yyyy}��5    #     5!      $1 +Количество автомобилей��6%   $ * 5#   #   $
 True��5$   #    5"   #   �� *Cars+5$   #     5%      $- 'Количество водителей��6%   $ * 5'   #   $
 True��5(   #    5&   #   �� *	Drivers+5(   #   5*      5+      5,      