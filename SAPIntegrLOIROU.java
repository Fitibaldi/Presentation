package com.technoclass.dmlwservice;


import com.technoclass.saptypes.loirou.LOIROU03;
import com.technoclass.saptypes.loirou.LOIROU03E1MAPAL;
import com.technoclass.saptypes.loirou.LOIROU03E1MAPLL;
import com.technoclass.saptypes.loirou.LOIROU03E1PLFLL;
import com.technoclass.saptypes.loirou.LOIROU03E1PLKOL;
import com.technoclass.saptypes.loirou.LOIROU03E1PLPOL;

import java.sql.ResultSet;
import java.sql.SQLException;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import javax.jws.WebMethod;
import javax.jws.WebResult;
import javax.jws.WebService;

import oracle.jdbc.OraclePreparedStatement;
import oracle.jdbc.OracleTypes;

import ru.dkc.mes.Dt_Response;


@WebService
public class SAPIntegrLOIROU {
    public SAPIntegrLOIROU() {
        super();
    }

    @WebMethod
    @WebResult(name = "doLOIROUResponse")
    public LOIROU03 doLOIROU(LOIROU03 loirou) {
        System.out.println("============ LOIROU ============");
        String xmlData = "";
        boolean isFirst = true;
        boolean hasPal = false;
        boolean hasKol = false;
        boolean hasFll = false;
        boolean hasPol = false;

        DBConnector db = new DBConnector();
        db.open();
        Integer idInputTable = null;
        String idStatus = null;
        Dt_Response resp = new Dt_Response();

        resp.setRCVPRN(loirou.getIDOC().getEDIDC40().getRCVPRN());
        resp.setMESTYP(loirou.getIDOC().getEDIDC40().getMESTYP());
        resp.setIDOCTP(loirou.getIDOC().getEDIDC40().getIDOCTYP());
        resp.setDOCNUM(loirou.getIDOC().getEDIDC40().getDOCNUM());

        List<TCLOIROU> llr = new ArrayList<TCLOIROU>();

        LOIROU03E1MAPLL e1mapll = loirou.getIDOC().getE1MAPLL();

        TCLOIROU lrpll = new TCLOIROU();
        lrpll.setMATNR(e1mapll.getMATNR());
        lrpll.setPLNNR(e1mapll.getPLNNR());
        lrpll.setWERKS(e1mapll.getWERKS());
        lrpll.setMSGFN(e1mapll.getMSGFN());

        for (Iterator<LOIROU03E1MAPAL> pal = e1mapll.getE1MAPAL().iterator();
             pal.hasNext(); ) {
            LOIROU03E1MAPAL e1mapal = pal.next();

            TCLOIROU lrpal = new TCLOIROU(lrpll);
            lrpal.setPLNAL(e1mapal.getPLNAL());

            hasPal = true;
            hasKol = false;
            hasFll = false;
            hasPol = false;

            for (Iterator<LOIROU03E1PLKOL> kol =
                 e1mapal.getE1PLKOL().iterator(); kol.hasNext(); ) {
                LOIROU03E1PLKOL e1plkol = kol.next();

                TCLOIROU lrkol = new TCLOIROU(lrpal);

                lrkol.setKTEXT(e1plkol.getKTEXT());
                lrkol.setPLNME(e1plkol.getPLNME());
                lrkol.setDATUB(e1plkol.getDATUB());
                lrkol.setDATUV(e1plkol.getDATUV());

                hasKol = true;
                hasFll = false;
                hasPol = false;

                for (Iterator<LOIROU03E1PLFLL> fll =
                     e1plkol.getE1PLFLL().iterator(); fll.hasNext(); ) {
                    LOIROU03E1PLFLL e1plfll = fll.next();

                    TCLOIROU lrfll = new TCLOIROU(lrkol);

                    lrfll.setPLNFL(e1plfll.getPLNFL());

                    hasFll = true;
                    hasPol = false;
                    for (Iterator<LOIROU03E1PLPOL> pol =
                         e1plfll.getE1PLPOL().iterator(); pol.hasNext(); ) {
                        LOIROU03E1PLPOL e1plpol = pol.next();

                        TCLOIROU lr = new TCLOIROU(lrfll);

                        lr.setVORNR(e1plpol.getVORNR());
                        lr.setBMSCH(e1plpol.getBMSCH());
                        lr.setLAR01(e1plpol.getLAR01());
                        lr.setMEINH(e1plpol.getMEINH());
                        lr.setVGE01(e1plpol.getVGE01());
                        lr.setVGW01(e1plpol.getVGW01());
                        lr.setARBID(e1plpol.getARBID());

                        hasPol = true;

                        llr.add(lr);
                    }
                    if (!hasPol) {
                        llr.add(lrfll);
                    }
                }
                if (!hasFll) {
                    llr.add(lrkol);
                }
            }
            if (!hasKol) {
                llr.add(lrpal);
            }
        }
        if (!hasPol) {
            llr.add(lrpll);
        }

        try {
            for (Iterator<TCLOIROU> lriter = llr.iterator(); lriter.hasNext();
            ) {
                TCLOIROU lr = new TCLOIROU();
                lr = lriter.next();

                OraclePreparedStatement psInsert;
                psInsert =
                        (OraclePreparedStatement)db.conn.prepareStatement("INSERT INTO SAP_INTEGR_TK (\n" +
                            "    DOC_TYPE, MATNR, PLNNR,\n" +
                            "    WERKS, PLNAL, KTEXT,\n" +
                            "    PLNME, PLNFL, VORNR,\n" +
                            "    BMSCH, LAR01, MEINH,\n" +
                            "    VGE01, VGW01, ARBID,\n" +
                            "    ID, DATUB, DATUV,\n" +
                "    DOCNUM, MSGFN)\n" +
                            "VALUES (?, ?, ?, " + "       ?, ?, ?, " +
                            "       ?, ?, ?, " + "       ?, ?, ?," +
                            "       ?, ?, ?," +
                            "?, TO_DATE (?, 'YYYYMMDD'), TO_DATE (?, 'YYYYMMDD'), " +
                "?, ?)" + " returning id into ?");

                psInsert.setString(1, "TK");
                psInsert.setString(2, lr.getMATNR());
                psInsert.setString(3, lr.getPLNNR());
                psInsert.setString(4, lr.getWERKS());
                psInsert.setString(5, lr.getPLNAL());
                psInsert.setString(6, lr.getKTEXT());
                psInsert.setString(7, lr.getPLNME());
                psInsert.setString(8, lr.getPLNFL());
                psInsert.setString(9, lr.getVORNR());
                psInsert.setString(10, lr.getBMSCH());
                psInsert.setString(11, lr.getLAR01());
                psInsert.setString(12, lr.getMEINH());
                psInsert.setString(13, lr.getVGE01());
                psInsert.setString(14, lr.getVGW01());
                psInsert.setString(15, lr.getARBID());
                if (!isFirst) {
                    psInsert.setInt(16, idInputTable);
                } else {
                    psInsert.setNull(16, OracleTypes.INTEGER);
                    isFirst = false;
                }
                psInsert.setString(17, lr.getDATUB());
                psInsert.setString(18, lr.getDATUV());
                psInsert.setString(19, resp.getDOCNUM());
                psInsert.setString(20, lr.getMSGFN());
                psInsert.registerReturnParameter(21, OracleTypes.NUMBER);
                psInsert.execute();
                ResultSet rset = psInsert.getReturnResultSet();
                rset.next();
                idInputTable = rset.getInt(1);
                psInsert.close();
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            db.close();
        }

        if (idInputTable != null && idInputTable > 0) {
            resp.setRETNUM(idInputTable.toString());
            SAPIntegration sap = new SAPIntegration();
            String respText = "";
            idStatus = sap.doSAP("TK", idInputTable);
            resp = sap.getMESResp(resp, "TK", idStatus);
        }

        try {
            com.technoclass.wsproxy.mesrespv2.HTTP_PortClient respPort =
                new com.technoclass.wsproxy.mesrespv2.HTTP_PortClient();
            respPort.main(resp);
        } catch (Exception e) {
            e.printStackTrace();
        }

        return (loirou);
    }

    public class TCLOIROU {
        public TCLOIROU() {
            super();
        }

        public TCLOIROU(TCLOIROU init) {
            ARBID = init.getARBID();
            BMSCH = init.getBMSCH();
            KTEXT = init.getKTEXT();
            LAR01 = init.getLAR01();
            MATNR = init.getMATNR();
            MEINH = init.getMEINH();
            PLNAL = init.getPLNAL();
            PLNFL = init.getPLNFL();
            PLNME = init.getPLNME();
            PLNNR = init.getPLNNR();
            VGE01 = init.getVGE01();
            VGW01 = init.getVGW01();
            VORNR = init.getVORNR();
            WERKS = init.getWERKS();
            DATUB = init.getDATUB();
            DATUV = init.getDATUV();
            MSGFN = init.getMSGFN();
        }

        private String ARBID;
        private String BMSCH;
        private String KTEXT;
        private String LAR01;
        private String MATNR;
        private String MEINH;
        private String PLNAL;
        private String PLNFL;
        private String PLNME;
        private String PLNNR;
        private String VGE01;
        private String VGW01;
        private String VORNR;
        private String WERKS;
        private String DATUB;
        private String DATUV;
        private String MSGFN;

        public void setARBID(String ARBID) {
            this.ARBID = ARBID;
        }

        public String getARBID() {
            return ARBID;
        }

        public void setBMSCH(String BMSCH) {
            this.BMSCH = BMSCH;
        }

        public String getBMSCH() {
            return BMSCH;
        }

        public void setKTEXT(String KTEXT) {
            this.KTEXT = KTEXT;
        }

        public String getKTEXT() {
            return KTEXT;
        }

        public void setLAR01(String LAR01) {
            this.LAR01 = LAR01;
        }

        public String getLAR01() {
            return LAR01;
        }

        public void setMATNR(String MATNR) {
            this.MATNR = MATNR;
        }

        public String getMATNR() {
            return MATNR;
        }

        public void setMEINH(String MEINH) {
            this.MEINH = MEINH;
        }

        public String getMEINH() {
            return MEINH;
        }

        public void setPLNAL(String PLNAL) {
            this.PLNAL = PLNAL;
        }

        public String getPLNAL() {
            return PLNAL;
        }

        public void setPLNFL(String PLNFL) {
            this.PLNFL = PLNFL;
        }

        public String getPLNFL() {
            return PLNFL;
        }

        public void setPLNME(String PLNME) {
            this.PLNME = PLNME;
        }

        public String getPLNME() {
            return PLNME;
        }

        public void setPLNNR(String PLNNR) {
            this.PLNNR = PLNNR;
        }

        public String getPLNNR() {
            return PLNNR;
        }

        public void setVGE01(String VGE01) {
            this.VGE01 = VGE01;
        }

        public String getVGE01() {
            return VGE01;
        }

        public void setVGW01(String VGW01) {
            this.VGW01 = VGW01;
        }

        public String getVGW01() {
            return VGW01;
        }

        public void setVORNR(String VORNR) {
            this.VORNR = VORNR;
        }

        public String getVORNR() {
            return VORNR;
        }

        public void setWERKS(String WERKS) {
            this.WERKS = WERKS;
        }

        public String getWERKS() {
            return WERKS;
        }

        public void setDATUB(String ATUB) {
            this.DATUB = ATUB;
        }

        public String getDATUB() {
            return DATUB;
        }

        public void setDATUV(String DATUV) {
            this.DATUV = DATUV;
        }

        public String getDATUV() {
            return DATUV;
        }

        public void setMSGFN(String MSGFN) {
            this.MSGFN = MSGFN;
        }

        public String getMSGFN() {
            return MSGFN;
        }
    }
}
