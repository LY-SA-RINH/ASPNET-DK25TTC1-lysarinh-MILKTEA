document.addEventListener("DOMContentLoaded", function () {
    const cacNutHienMatKhau =
        document.querySelectorAll(".password-toggle");

    cacNutHienMatKhau.forEach(function (nut) {
        nut.addEventListener("click", function () {
            const idOTrong = nut.getAttribute("data-target");
            const oMatKhau = document.getElementById(idOTrong);

            if (!oMatKhau) {
                return;
            }

            const dangAnMatKhau =
                oMatKhau.type === "password";

            oMatKhau.type =
                dangAnMatKhau ? "text" : "password";

            const bieuTuong = nut.querySelector("i");

            if (bieuTuong) {
                bieuTuong.className =
                    dangAnMatKhau
                        ? "bi bi-eye-slash"
                        : "bi bi-eye";
            }

            nut.title =
                dangAnMatKhau
                    ? "Ẩn mật khẩu"
                    : "Hiện mật khẩu";

            nut.setAttribute(
                "aria-label",
                nut.title);
        });
    });
});