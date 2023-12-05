# NOTE

## 20/11/2023
- Kéo thả module bị ra ngoài vùng môi trường //DONE 
- Chưa genarate được tọa độ module khi user không nhập vào //DONE
- Bug: config từ lần thứ 2 trở đi không lưu được 
- chưa có chức năng read config từ hardware
## 21/11/2023
- Tạo dropdown list cho user chọn module //REMOVE
- Tạo dropdown list "File", bao gồm các chức năng: New, Open, Save, Save as, Exit
- sửa delete //DONE
- sửa selectedhistoryitem //DONE
- realtime binding //DONE
- startPort xong thì add luôn vào serial port, như vậy là sai -> fixx //DONE
- sửa lại save button khi mà update //DONE
- khi mà run thì chỉ có thể xem module, không thể thao tác update xóa // --
- khi config phải xét xem ở mode bao nhiêu
- không có device thì không được phép run //DONE
- pause lại để coi parameter //DONE
- close sau khi delete ko reset parameters //DONE
- check mode khi run 
- save cần phải save luôn lịch sử data không 
- liệt kê tình huống nào thì sẽ phải save, nếu không mất data


## 30/11/2023
- Khi stop, nhấn config thì không được, bị lỗi -> stop sẽ clear history data, còn pause thì không
- tạo nút clear data 
- Khi khởi tạo device, config bị lỗi, xuất hiện exception liên tục

## 04/12/2023
- clear buffer cua hw sau khi nhan data tu node controller

## 05/12/2023
- sửa UI phần fixmode, thông tin hiện ở packet send là address và channel của device đích, không phải device source


