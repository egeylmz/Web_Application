// Ekipman Talebi Onaylama ve Reddetme için AJAX işlevleri

let ekipmanTalepIslemId = null;
let ekipmanTalepIslemKisi = null;
let ekipmanTalepIslemAdi = null;

function showEkipmanTalepOnayModal(id, talepKisi, ekipmanAdi) {
    ekipmanTalepIslemId = id;
    ekipmanTalepIslemKisi = talepKisi;
    ekipmanTalepIslemAdi = ekipmanAdi;
    
    document.getElementById('ekipmanTalepOnayMesaj').innerHTML = 
        `<strong>${talepKisi}</strong> adlı kullanıcının <strong>${ekipmanAdi}</strong> ekipman talebini onaylamak istediğinize emin misiniz?`;
    document.getElementById('ekipmanTalepOnayModal').style.display = 'flex';
}

function showEkipmanTalepReddetModal(id, talepKisi, ekipmanAdi) {
    ekipmanTalepIslemId = id;
    ekipmanTalepIslemKisi = talepKisi;
    ekipmanTalepIslemAdi = ekipmanAdi;
    
    document.getElementById('ekipmanTalepReddetMesaj').innerHTML = 
        `<strong>${talepKisi}</strong> adlı kullanıcının <strong>${ekipmanAdi}</strong> ekipman talebini reddetmek istediğinize emin misiniz?`;
    document.getElementById('ekipmanTalepReddetModal').style.display = 'flex';
}

function ekipmanTalepOnayla() {
    if (ekipmanTalepIslemId) {
        fetch('/Admin/TalepOnayla', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: 'id=' + encodeURIComponent(ekipmanTalepIslemId)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/EkipmanTalepleri')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#ekipmanTalepTabloContainer');
                        if (newTable) {
                            document.getElementById('ekipmanTalepTabloContainer').innerHTML = newTable.innerHTML;
                        }
                    });
                
                // Modal'ı kapat
                document.getElementById('ekipmanTalepOnayModal').style.display = 'none';
                ekipmanTalepIslemId = null;
            } else {
                alert(data.message || 'Onaylama işlemi başarısız.');
            }
        })
        .catch(error => {
            alert('Bir hata oluştu: ' + error.message);
        });
    }
}

function ekipmanTalepReddet() {
    if (ekipmanTalepIslemId) {
        fetch('/Admin/TalepReddet', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: 'id=' + encodeURIComponent(ekipmanTalepIslemId)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/EkipmanTalepleri')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#ekipmanTalepTabloContainer');
                        if (newTable) {
                            document.getElementById('ekipmanTalepTabloContainer').innerHTML = newTable.innerHTML;
                        }
                    });
                
                // Modal'ı kapat
                document.getElementById('ekipmanTalepReddetModal').style.display = 'none';
                ekipmanTalepIslemId = null;
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
    document.getElementById('ekipmanTalepOnayIptalBtn').onclick = function() {
        document.getElementById('ekipmanTalepOnayModal').style.display = 'none';
        ekipmanTalepIslemId = null;
    };
    
    document.getElementById('ekipmanTalepReddetIptalBtn').onclick = function() {
        document.getElementById('ekipmanTalepReddetModal').style.display = 'none';
        ekipmanTalepIslemId = null;
    };
    
    document.getElementById('ekipmanTalepOnayBtn').onclick = ekipmanTalepOnayla;
    document.getElementById('ekipmanTalepReddetBtn').onclick = ekipmanTalepReddet;
};
