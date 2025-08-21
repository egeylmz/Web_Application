// Kullanıcı Silmek için onaylama modalı

let silinecekId = null;
window.showSilmeModal = function(id) {
    silinecekId = id;
    document.getElementById('silmeModal').style.display = 'flex';
};

window.showKullaniciEkleModal = function() {
    document.getElementById('kullaniciEkleModal').style.display = 'flex';      // flex ile ekranda gözükür.         
    document.getElementById('kullaniciEkleHata').style.display = 'none';
};

window.onload = function() {
    // Kullanıcı silme modal işlemleri
    document.getElementById('modalIptalBtn').onclick = function() {         // Modal'ı kapat
        document.getElementById('silmeModal').style.display = 'none';
        silinecekId = null;
    };
    document.getElementById('modalOnayBtn').onclick = function() {          //eğer onaylanırsa SilKullanici adresine POST isteği gönderilecek.
        if (silinecekId) {
            fetch('/Admin/SilKullanici', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',                //form verisi formatı
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value        //koruma için
                },
                body: 'id=' + encodeURIComponent(silinecekId)
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // sayfa fetch ile tekrar yüklenecek.
                    fetch('/Admin/Kullanicilar')
                        .then(resp => resp.text())
                        .then(html => {
                            const parser = new DOMParser();
                            const doc = parser.parseFromString(html, 'text/html');
                            const newTable = doc.querySelector('#kullaniciTabloContainer');         //sadece tablo içeriği alınacak.
                            document.getElementById('kullaniciTabloContainer').innerHTML = newTable.innerHTML;
                        });
                    document.getElementById('silmeModal').style.display = 'none';
                    silinecekId = null;
                } else {
                    alert(data.message || 'Silme işlemi başarısız.');
                }
            });
        }
    };

    // Kullanıcı ekleme modal işlemleri
    document.getElementById('kullaniciEkleIptalBtn').onclick = function() {
        document.getElementById('kullaniciEkleModal').style.display = 'none';
    };

    document.getElementById('kullaniciEkleForm').onsubmit = function(e) {
        e.preventDefault();
        var form = e.target;
        var formData = new FormData(form);
        
        // Checkbox değerini kontrol et
        formData.set('Aktif', form.Aktif.checked);

        fetch('/Admin/YeniKullanici', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/Kullanicilar')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#kullaniciTabloContainer');
                        document.getElementById('kullaniciTabloContainer').innerHTML = newTable.innerHTML;
                    });
                
                // Modal'ı kapat
                document.getElementById('kullaniciEkleModal').style.display = 'none';
                
                // Form'u temizle
                form.reset();
            } else {
                var hataDiv = document.getElementById('kullaniciEkleHata');
                hataDiv.textContent = "Kayıt başarısız: " + (data.message || 'Bilinmeyen hata');
                hataDiv.style.display = 'block';
            }
        })
        .catch(error => {
            var hataDiv = document.getElementById('kullaniciEkleHata');
            hataDiv.textContent = "Kayıt başarısız: " + error.message;
            hataDiv.style.display = 'block';
        });
    };
};