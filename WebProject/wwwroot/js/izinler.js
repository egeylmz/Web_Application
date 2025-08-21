
// İzin Onaylama ve Reddetme için AJAX işlevleri

let izinIslemId = null;
let izinIslemAdSoyad = null;
let izinIslemTipi = null;

function showIzinOnayModal(id, adSoyad, izinTipi) {
    izinIslemId = id;
    izinIslemAdSoyad = adSoyad;
    izinIslemTipi = izinTipi;
    
    document.getElementById('izinOnayMesaj').innerHTML = 
        `<strong>${adSoyad}</strong> adlı kullanıcının <strong>${izinTipi}</strong> izin talebini onaylamak istediğinize emin misiniz?`;
    document.getElementById('izinOnayModal').style.display = 'flex';
}

function showIzinReddetModal(id, adSoyad, izinTipi) {
    izinIslemId = id;
    izinIslemAdSoyad = adSoyad;
    izinIslemTipi = izinTipi;
    
    document.getElementById('izinReddetMesaj').innerHTML = 
        `<strong>${adSoyad}</strong> adlı kullanıcının <strong>${izinTipi}</strong> izin talebini reddetmek istediğinize emin misiniz?`;
    document.getElementById('izinReddetModal').style.display = 'flex';
}

function izinOnayla() {
    if (izinIslemId) {
        fetch('/Admin/IzinOnayla', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: 'id=' + encodeURIComponent(izinIslemId)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/IzinTalepleri')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#izinTabloContainer');
                        if (newTable) {
                            document.getElementById('izinTabloContainer').innerHTML = newTable.innerHTML;
                        }
                    });
                
                // Modal'ı kapat
                document.getElementById('izinOnayModal').style.display = 'none';
                izinIslemId = null;
            } else {
                alert(data.message || 'Onaylama işlemi başarısız.');
            }
        })
        .catch(error => {
            alert('Bir hata oluştu: ' + error.message);
        });
    }
}

function izinReddet() {
    if (izinIslemId) {
        fetch('/Admin/IzinReddet', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: 'id=' + encodeURIComponent(izinIslemId)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/IzinTalepleri')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#izinTabloContainer');
                        if (newTable) {
                            document.getElementById('izinTabloContainer').innerHTML = newTable.innerHTML;
                        }
                    });
                
                // Modal'ı kapat
                document.getElementById('izinReddetModal').style.display = 'none';
                izinIslemId = null;
            } else {
                alert(data.message || 'Reddetme işlemi başarısız.');
            }
        })
        .catch(error => {
            alert('Bir hata oluştu: ' + error.message);
        });
    }
}

// Modal kapatma işlevleri
window.onload = function() {
    document.getElementById('izinOnayIptalBtn').onclick = function() {
        document.getElementById('izinOnayModal').style.display = 'none';
        izinIslemId = null;
    };
    
    document.getElementById('izinReddetIptalBtn').onclick = function() {
        document.getElementById('izinReddetModal').style.display = 'none';
        izinIslemId = null;
    };
    
    document.getElementById('izinOnayBtn').onclick = izinOnayla;
    document.getElementById('izinReddetBtn').onclick = izinReddet;
};
